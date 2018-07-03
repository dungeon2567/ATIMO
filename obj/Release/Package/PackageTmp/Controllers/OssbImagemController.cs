using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using ATIMO;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

namespace Atimo.Controllers
{

    public class OssbImagemController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /OssbServico/

        public ActionResult Index(int id = 0)
        {
            var viewModel = new OssbImagemViewModel()
            {
                Ossb = id,
                Items = _db
                .OSSB_ALBUM
                .Include(ab => ab.OSSB_IMAGEM)
                .Where(ab => ab.OSSB == id)
                 .ToArray()
            };

            return View(viewModel);
        }

        public async Task<FileResult> GetImagem(Int32 id)
        {
            var imagem = await _db.OSSB_IMAGEM
                 .Include(om => om.ANEXO1)
                 .FirstOrDefaultAsync(img => img.ID == id);

            if (imagem != null)
            {
                return File(imagem.ANEXO1.BUFFER, MimeMapping.GetMimeMapping(imagem.ANEXO1.NOME), imagem.ANEXO1.NOME);
            }
            else
                return null;
        }

        public async Task<JsonResult> PermutarImagens(Int32 a, Int32 b)
        {
            var imagemA = await _db.OSSB_IMAGEM
                 .FirstOrDefaultAsync(img => img.ID == a);

            var imagemB = await _db.OSSB_IMAGEM
                 .FirstOrDefaultAsync(img => img.ID == b);

            if (imagemA != null && imagemB != null)
            {
                var a_anexo = imagemA.ANEXO;
                var a_desc = imagemA.DESCRICAO;

                imagemA.ANEXO = imagemB.ANEXO;
                imagemA.DESCRICAO = imagemB.DESCRICAO;

                imagemB.ANEXO = a_anexo;
                imagemB.DESCRICAO = a_desc;

                await _db.SaveChangesAsync();

                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = 1}, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> UpdateAlbumDesc(Int32 id, string desc)
        {

                var album = await _db
                    .OSSB_ALBUM
                    .FirstOrDefaultAsync(ab => ab.ID == id);

                if (album == null)
                    return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

                album.DESCRICAO = desc;

                _db.Entry(album)
                    .State = EntityState.Modified;

                await _db.SaveChangesAsync();

                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> UpdateImageDesc(Int32 id, string desc)
        {

                var imagem = await _db
                    .OSSB_IMAGEM
                    .FirstOrDefaultAsync(img => img.ID == id);

                if (imagem == null)
                    return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

                imagem.DESCRICAO = desc;

                _db.Entry(imagem)
                    .State = EntityState.Modified;

                await _db.SaveChangesAsync();

                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> DeleteImage(Int32 id)
        {
                var image = await _db
                  .OSSB_IMAGEM
                   .Include(img => img.ANEXO1)
                    .FirstOrDefaultAsync(img => img.ID == id);

                if (image == null)
                    return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

                _db.Entry(image.ANEXO1)
                    .State = EntityState.Deleted;

                _db.Entry(image)
                    .State = EntityState.Deleted;

                await _db.SaveChangesAsync();

                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> DeleteAlbum(Int32 id)
        {
                var album = await _db
                  .OSSB_ALBUM
                   .Include(ab => ab.OSSB_IMAGEM)
                   .Include(ab => ab.OSSB_IMAGEM.Select(img => img.ANEXO1))
                    .FirstOrDefaultAsync(ab => ab.ID == id);

                if (album == null)
                    return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

                foreach (var img in album.OSSB_IMAGEM)
                {
                    _db.Entry(img.ANEXO1)
                        .State = EntityState.Deleted;

                    _db.Entry(img)
                        .State = EntityState.Deleted;

                }

                _db.Entry(album)
                    .State = EntityState.Deleted;


                await _db.SaveChangesAsync();

                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FileUpload(Int32 ossb, IEnumerable<HttpPostedFileBase> files)
        {
            if (files.Any())
            {
                var album = new OSSB_ALBUM()
                {
                    OSSB = ossb,
                    DESCRICAO = "",
                };

                foreach (var file in files)
                {
                    if (file != null)
                    {
                        String filename = Path.GetFileName(file.FileName);

                        byte[] data = new byte[file.InputStream.Length];

                        await file.InputStream.ReadAsync(data, 0, (int)file.InputStream.Length);

                        album.OSSB_IMAGEM.Add(new OSSB_IMAGEM() { DESCRICAO = DateTime.Today.ToString("dd/MM/yyyy") + " - ", ANEXO1 = new ANEXO() { BUFFER = data, NOME = file.FileName } });
                    }
                }

                _db.OSSB_ALBUM.Add(album);

                await _db.SaveChangesAsync();


            }

            return Redirect(Request.UrlReferrer.ToString());
        }




        [HttpPost]
        public async Task<ActionResult> FileUploadAlbum(Int32 album, IEnumerable<HttpPostedFileBase> files)
        {
            if (files.Any())
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        String filename = Path.GetFileName(file.FileName);

                        byte[] data = new byte[file.InputStream.Length];

                        await file.InputStream.ReadAsync(data, 0, (int)file.InputStream.Length);

                        _db.OSSB_IMAGEM.Add(new OSSB_IMAGEM() { ALBUM = album, DESCRICAO = DateTime.Today.ToString("dd/MM/yyyy") + " - ", ANEXO1 = new ANEXO() { BUFFER = data, NOME = file.FileName } });
                    }
                }

                await _db.SaveChangesAsync();


            }

            return Redirect(Request.UrlReferrer.ToString());
        }


        sealed class PageEventHandler : PdfPageEventHelper
        {
            public Controller Controller;

            public PageEventHandler(Controller controller)
            {
                Controller = controller;
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                var cb = writer.DirectContent;

                Font normalFont = FontFactory.GetFont(FontFactory.TIMES, 9);

                Font boldFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 9);

                float y = document.PageSize.GetBottom(document.BottomMargin);

                cb.MoveTo(document.LeftMargin, y);

                cb.SetColorStroke(new BaseColor(98, 36, 35));

                cb.LineTo(document.PageSize.GetRight(document.RightMargin), y);

                cb.Stroke();

                String[] texts = new String[]
                {
                    "São Bento Engenharia | Tel. +55 11 3331-1897",
                    "Rua Ipojuca, 54 | CEP 03304-050 – São Paulo – SP",
                    "ATENDIMENTO@SAOBENTOSERVICOS.COM.BR",
                    "WWW.SAOBENTOSERVICOS.COM.BR"
                };

                Font[] fonts = new Font[]
                {
                    boldFont,normalFont,boldFont,boldFont
                };

                y -= 10;

                for (Int32 it = 0; it < 4; ++it)
                {
                    String text = texts[it];
                    Font font = fonts[it];

                    y -= 2;

                    cb.BeginText();

                    cb.SetFontAndSize(font.BaseFont, 9);

                    float x = document.LeftMargin + (document.PageSize.GetRight(document.RightMargin) - document.LeftMargin) / 2 - (font.BaseFont.GetWidthPoint(text, 9) / 2);

                    cb.SetTextMatrix(x, y);

                    cb.ShowText(text);

                    cb.EndText();

                    y -= font.Size;
                }

            }

        }

        public static IEnumerable<IEnumerable<T>> TakeChunks<T>(IEnumerable<T> source, int size)
        {

            var list = new List<T>(size);

            foreach (T item in source)
            {
                list.Add(item);

                if (list.Count == size)
                {
                    List<T> chunk = list;
                    list = new List<T>(size);
                    yield return chunk;
                }
            }

            if (list.Count > 0)
            {
                yield return list;
            }
        }

        public FileContentResult GetRelatorioPDF(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                OSSB ossb = _db.OSSB
                    .Include(o => o.PESSOA)
                    .Include(o => o.PROJETO1)
                    .Include(o => o.CONTRATO1)
                    .FirstOrDefault(o => o.ID == id);

                if (ossb != null)
                {
                    try
                    {
                        var fs = new MemoryStream();

                        Document doc = new Document(PageSize.A4, 25, 25, 30, 65);

                        PdfWriter writer = PdfWriter.GetInstance(doc, fs);

                        var capturarBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/Capturar.PNG"));
                        var backgroundBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/background.png"));

                        writer.PageEvent = new PageEventHandler(this);

                        var albuns = _db.OSSB_ALBUM
                            .Include(ab => ab.OSSB_IMAGEM)
                            .Include(ab => ab.OSSB_IMAGEM.Select(im => im.ANEXO1))
                            .Where(ab => ab.OSSB == id)
                            .ToArray();

                        doc.Open();

                        {
                            var normalFont = FontFactory.GetFont(FontFactory.TIMES, 10);
                            var normalDesc = FontFactory.GetFont(FontFactory.TIMES, 9);
                            var boldFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 10);
                            var titleBold = FontFactory.GetFont(FontFactory.TIMES_BOLD, 12);

                            {
                                {
                                    Image img = Image.GetInstance(capturarBytes, true);

                                    img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;

                                    doc.Add(img);
                                }


                                doc.Add(new LineSeparator() { LineColor = new BaseColor(98, 36, 35) });

                                {
                                    var cb = writer.DirectContentUnder;

                                    cb.SaveState();


                                    var img = Image.GetInstance(backgroundBytes, true);

                                    Rectangle r = new Rectangle(doc.PageSize);

                                    img.SetAbsolutePosition(doc.PageSize.GetRight(doc.RightMargin) - img.Right, doc.PageSize.GetBottom(doc.BottomMargin) - img.Bottom);

                                    cb.AddImage(img);

                                    cb.RestoreState();
                                }

                                doc.Add(new Paragraph(new Chunk("São Paulo SP, " + System.DateTime.Today.ToLongDateString(), normalFont)));
                                doc.Add(new Paragraph(Environment.NewLine));
                                doc.Add(new Paragraph(Environment.NewLine));

                                doc.Add(new Paragraph(new Chunk(ossb.PESSOA.RAZAO, normalFont)));

                                if (ossb.LOJA1 != null)
                                {
                                    doc.Add(new Paragraph(new Chunk(ossb.LOJA1.APELIDO, normalFont)));
                                    doc.Add(new Paragraph(new Chunk(ossb.LOJA1.ENDERECO + (String.IsNullOrEmpty(ossb.LOJA1.COMPLEMENTO) ? "" : (", " + ossb.LOJA1.COMPLEMENTO)), normalFont)));
                                }

                                doc.Add(new Paragraph(new Chunk("OS: " + ossb.ID, normalFont)));
                                doc.Add(new Paragraph(new Chunk("STATUS: " + ossb.TEXTO_SITUACAO, normalFont)));



                                doc.Add(new Paragraph(Environment.NewLine));

                                var phrase = new Phrase();

                                phrase.Add(new Chunk("Estamos apresentando nosso ", normalFont));
                                phrase.Add(new Chunk("RELATORIO FOTOGRAFICO", boldFont));
                                phrase.Add(new Chunk(" referente à execução dos serviços em pauta.", normalFont));

                                doc.Add(phrase);

                                doc.Add(new Paragraph(Environment.NewLine));

                                doc.Add(new Paragraph(ossb.OCORRENCIA, normalFont));

                                doc.Add(new Paragraph(Environment.NewLine));

                                phrase = new Phrase();

                                phrase.Add(new Chunk("A nossa atuação será desenvolvida com base nos preceitos estabelecidos na ", normalFont));
                                phrase.Add(new Chunk("POLÍTICA DA QUALIDADE da SÃO BENTO ENGENHARIA", boldFont));
                                phrase.Add(new Chunk(" e as atividades de manutenção serão executadas em conformidade com os Procedimentos de Engenharia e Manutenção, desenvolvidos pela ", normalFont));
                                phrase.Add(new Chunk("SÃO BENTO ENGENHARIA", boldFont));
                                phrase.Add(new Chunk(", ao longo de anos de atuação no mercado.", normalFont));

                                doc.Add(phrase);

                                doc.Add(new Paragraph(Environment.NewLine));

                                doc.Add(new Paragraph(new Chunk("Colocamo-nos à disposição para quaisquer esclarecimentos necessários.", normalFont)) { Alignment = Element.ALIGN_CENTER });

                            }


                            foreach (var album in albuns)
                            {
                                foreach (var imageSplit in TakeChunks(album.OSSB_IMAGEM, 4))
                                {
                                    doc.NewPage();

                                    {
                                        Image img = Image.GetInstance(capturarBytes, true);

                                        img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;

                                        doc.Add(img);

                                        doc.Add(new LineSeparator() { LineColor = new BaseColor(98, 36, 35) });
                                    }

                                    PdfPTable table;

                                    if (imageSplit.Count() == 1)
                                    {
                                        table = new PdfPTable(imageSplit.Count());
                                    }
                                    else
                                    {
                                        table = new PdfPTable(2);
                                    }

                                    table.WidthPercentage = 100;



                                    foreach (var image in imageSplit)
                                    {

                   
                                        var cell = new PdfPCell()
                                        {
                                            BorderWidth = 1,
                                            Padding = 3
                                        };

      

                                        Image img = Image.GetInstance(image.ANEXO1.BUFFER, true);
                                        img.ScaleToFit(250, 200);

                                        img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                                        cell.AddElement(img);

                                        var p = new Paragraph(new Chunk(image.DESCRICAO, normalDesc));

                                        p.Alignment = Element.ALIGN_CENTER;

                                        cell.AddElement(p);

                                        cell.FixedHeight = 300;

                                        table.AddCell(cell);
                                    }

                                    if(imageSplit.Count() % 2 == 1 && imageSplit.Count() > 1)
                                    {
                                        table.AddCell(new PdfPCell());
                                    }

                                    doc.Add(table);
                                }

                                if (!String.IsNullOrWhiteSpace(album.DESCRICAO))
                                {
                                    doc.NewPage();

                                    {
                                        Image img = Image.GetInstance(capturarBytes, true);

                                        img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;

                                        doc.Add(img);

                                        doc.Add(new LineSeparator() { LineColor = new BaseColor(98, 36, 35) });
                                    }

                                    doc.Add(new Paragraph(new Chunk("CONCLUSÃO", titleBold)) { Alignment = Element.ALIGN_CENTER });

                                    doc.Add(new Paragraph(new Chunk(album.DESCRICAO, normalDesc)) { Alignment = Element.ALIGN_CENTER });
                                }
                            }
                        }

                        doc.Close();
                        writer.Close();

                        Response.AppendHeader("Content-Disposition", "inline; filename=orcamento.pdf;");

                        return new FileContentResult(fs.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                return null;
            }
            else
                return null;
        }
    }
}

