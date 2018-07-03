using ATIMO.Models.Faturamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Helpers.ModelsUtils
{
    public class MunicipioHelper
    {
        public static string getCodigoIbgeFromCidade(string astrNomeCidade)
        {
            try
            {
                FATURAMENTOEntities m_DbContext = new FATURAMENTOEntities();

                string l_CodIBGE = m_DbContext.Municipios.Where(p => p.Nome == astrNomeCidade).Select(p => p.CodigoIBGE).FirstOrDefault();
                
                return l_CodIBGE; //return "3550308";
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public static string getNomeCidadeFromCodigoIbge(string astrCodigoIBGE)
        {
            try
            {
                FATURAMENTOEntities m_DbContext = new FATURAMENTOEntities();

                string l_CodIBGE = m_DbContext.Municipios.Where(p => p.CodigoIBGE == astrCodigoIBGE).Select(p => p.Nome).FirstOrDefault();

                return l_CodIBGE; //return "3550308";
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }
    }
}