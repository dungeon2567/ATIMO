using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.Models.Faturamento;
using Nfse.PMSP.RN;
using ATIMO.Helpers;

namespace Atimo.Controllers
{
    public class DimasController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult TesteWs()
        {
            string lstrAvisos;
            NFSePmspSender nfse = new NFSePmspSender();
            nfse.Empresa = getEmpresa(out lstrAvisos);
            return View(nfse);
        }
        
        [HttpPost]
        public ActionResult TesteWs(NFSePmspSender emp)
        {
            NFSePmspSender lobjNfe = new NFSePmspSender();
            IEnvioParam param = null;
            string lstrAvisoForn;

            switch (emp.Operacao)
            {
                case enmOperacaoNfsePmsp.eEnvioLote:
                case enmOperacaoNfsePmsp.eTesteEnvio:
                case enmOperacaoNfsePmsp.eCancelamentoNF:
                    param = new EnvioParamListaFaturamentos(emp.Operacao);
                    break;
                case enmOperacaoNfsePmsp.eConsultaRecebidasPeriodo:
                case enmOperacaoNfsePmsp.eConsultaEnviadaPeriodo:
                    param = new EnvioParamPeriodo(emp.Operacao, DateTime.Now.Date.AddMonths(-1), DateTime.Now.Date);
                    break;
                case enmOperacaoNfsePmsp.eConsultaCNPJ:
                    param = new EnvioParamConsultaCNPJ();
                    break;
                default:
                    break;
            }
            if (param != null)
                lobjNfe.iniciaEnvio(param, getEmpresa(out lstrAvisoForn), 1);
            else
                lobjNfe.Erros.Add("Nao foi definido o parametro de envio da NF");


            return View(lobjNfe);
        }


        public ActionResult CadEmpresaFromCertificado()
        {
            string lstrAvisos;
            return View(getEmpresa(out lstrAvisos));
        }

        public ActionResult DadosClientePorCnpj()
        {
            PESSOA lobjPessoa = new PESSOA();
            return View(lobjPessoa);
        }

        [HttpPost]
        public ActionResult DadosClientePorCnpj(PESSOA aobjPessoaParam)
        {
            List<string> llistOutAlert = new List<string>();
            try
            {
                aobjPessoaParam = Utils.getPessoaFromReceita_PMSP(aobjPessoaParam.NUM_DOC, ref llistOutAlert);
                aobjPessoaParam.NUM_DOC = Utils.LimpaCpfCnpj(aobjPessoaParam.NUM_DOC);
                aobjPessoaParam.OBSERVACAO = string.Join(", ", llistOutAlert.ToArray());
            }
            catch (Exception Erro)
            {
                aobjPessoaParam.NOME = Erro.Message;
            }

            return View(aobjPessoaParam);
        }

        public ActionResult CalculoRetencoes()
        {
            return View(new Faturamento());
        }

        [HttpPost]
        public ActionResult CalculoRetencoes(Faturamento aobjFaturamento)
        {
            
            PESSOA l_Cliente = _db.PESSOA.Find(aobjFaturamento.ClienteID);
            PessoaTomador lobjTomador = new PessoaTomador((enmTipoPessoaTrib)l_Cliente.TIPO_PESSOA_TRIBUTACAO.GetValueOrDefault((int)enmTipoPessoaTrib.eNaoPreenchido), l_Cliente.CIDADE);
            string lstrTemp;
            aobjFaturamento.Retencoes = FaturamentoRetencoes.getRetencoes(aobjFaturamento.ValorBruto, aobjFaturamento.ValorMaoDeObra, aobjFaturamento.EmpresaEmissao, ref lobjTomador, out lstrTemp);
            return View(aobjFaturamento);
        }


        private Empresa getEmpresa(out string astrAvisos)
        {
            NFSePmspSender nfse = new NFSePmspSender();
            Empresa lobjEmpresa = new Empresa();
            string lstrCertificado = Certificado.getCurrentSerial(out lobjEmpresa);
            lobjEmpresa.SerialCertificado = lstrCertificado;
            nfse.iniciaEnvio(new EnvioParamConsultaCNPJ(), lobjEmpresa, 1);
            lobjEmpresa.CcmPmsp = nfse.InscricaoMunicipalCCM;
            lobjEmpresa.EmiteNF = nfse.EmiteNF.ToString();
            astrAvisos = string.Empty;

            if (string.IsNullOrEmpty(lobjEmpresa.CcmPmsp) == false)
            {
                try
                {

                CadastroPessoaPMSP lobjCmmResult = ListResultadoCcm.getDadosFromPMSPByCcm(lobjEmpresa.CcmPmsp);
                if (lobjCmmResult.SituacaoCcm == enmSituacaoCcm.eCcmEncontrado)
                {
                    if (string.IsNullOrEmpty(lobjEmpresa.CNPJ))
                        lobjEmpresa.CNPJ = lobjCmmResult.CpfCnpj;

                    if (Utils.LimpaCpfCnpj(lobjEmpresa.CNPJ) == Utils.LimpaCpfCnpj(lobjCmmResult.CpfCnpj))
                    {
                        lobjEmpresa.ListaServicos = lobjCmmResult.Servicos;
                        if (lobjCmmResult.Servicos.Count >= 1)
                            lobjEmpresa.CodigoServicoPMSP = lobjCmmResult.Servicos[0].Codigo;
                        lobjEmpresa.Nome = lobjCmmResult.RazaoSocial;
                        switch (lobjCmmResult.TipoPessoa)
                        {
                            case "PJ SIMPLES":
                                lobjEmpresa.OptanteSimplesNacional = "S";
                                break;
                            case "PJ COMUM":
                                lobjEmpresa.OptanteSimplesNacional = "N";
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        astrAvisos = "O CNPJ encontrado no certificado não é o mesmo cadastrado na prefeitura, favor consulte o analista responsável";
                    }
                }
                else
                    astrAvisos = "Não foi encontrado um CCM válido, favor consulte o analista responsável";

                }
                catch (Exception Erro)
                {
                    astrAvisos = "Ocorreu o seguinte erro ao obter os dados da prefeitura " + Erro.Message;
                }
            }
            else
            {
                astrAvisos = "Devido a falta de identificação de CCM via web service da prefeitura, não é possível identificar demais dados";
            }

            return lobjEmpresa;
        }



    }

    
}
