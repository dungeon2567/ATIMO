using ATIMO.Helpers;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace ATIMO.Helpers.Uploader
{
	public class Uploader : IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public Uploader()
		{
		}

		public void ProcessRequest(HttpContext context)
		{
			
			string l_Filename = string.Empty;
			
			if (context.Request.QueryString["FileName"] != null)
			{
				l_Filename = context.Request.QueryString["FileName"];
				return;
			}
			string lstrReturn = "error";
			
			try
			{

                string l_FileExtension = ""; // Path.GetExtension(context.Request.Files[0].FileName).ToLower();
				string l_className = string.Empty;
				string l_classRelativePath = string.Empty;
				string l_param = string.Empty;
                string l_paramReturn = string.Empty;
				string l_diskPath = string.Empty;
				int l_width = 0;
				int l_height = 0;
                string l_WebRelativePath = string.Empty;
                string l_AppRelativePath = string.Empty;
                string l_AbsoluteUrl = string.Empty;
                string l_fullClassName = string.Empty;
                double l_FileSize = 0;

				if (!string.IsNullOrEmpty(context.Request.Form["className"])) {
					l_className = context.Request.Form["className"].ToString();
				}

				if (!string.IsNullOrEmpty(context.Request.Form["param"])) {
					l_param = context.Request.Form["param"].ToString();
				}

                if (!string.IsNullOrEmpty(context.Request.Form["param_return"])) {
                    l_paramReturn = context.Request.Form["param_return"].ToString();
                }

				if (!string.IsNullOrEmpty(context.Request.Form["newWidth"])) {
					int.TryParse(context.Request.Form["newWidth"], out l_width);
				}

				if (!string.IsNullOrEmpty(context.Request.Form["newHeight"])) {
					int.TryParse(context.Request.Form["newHeight"], out l_height);
				}

                string l_rootPath = "";
                string l_relativePath = "";
                int l_newHeight = 0;
                int l_newWidth = 0;
                string larrReturnFiles = "";

                //Incluído - Rafael - 03/10/2016 (Início)
                if (l_className.IndexOf(".") > -1)
                    l_fullClassName = l_className;
                else
                    l_fullClassName = string.Concat("ATIMO.Helpers.ModelsUtils.", l_className);
                                                    
                if (Utils.HasMethod(l_fullClassName, "GetRootDirectory"))
                {
                    if (string.IsNullOrEmpty(l_param))
                        l_rootPath = Utils.InvokeStringMethod(l_fullClassName, "GetRootDirectory");
                    else
                        l_rootPath = Utils.InvokeStringMethod(l_fullClassName, "GetRootDirectory", new string[] { l_param });
                    
                    l_rootPath = l_rootPath.TrimEnd('\\') + "\\";
                }


                
                if (string.IsNullOrEmpty(l_rootPath))
                    l_rootPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\";

                //Incluído - Rafael - 03/10/2016 (Fim   )

                for (int i = 0; i < context.Request.Files.Count; i++ )
                {
                    l_FileExtension = Path.GetExtension(context.Request.Files[i].FileName).ToLower();

                    l_Filename = Path.GetFileName(context.Request.Files[i].FileName);

                    l_FileSize = context.Request.Files[i].InputStream.Length / 1024.0;

                    byte[] numArray = new byte[checked((Int32)context.Request.Files[i].InputStream.Length)];
                    context.Request.Files[i].InputStream.Read(numArray, 0, (int)numArray.Length);
                    
                    l_relativePath = getRelativeFileStoragePath(l_Filename, l_className, l_param);

                    l_diskPath = string.Concat(l_rootPath, l_relativePath);

                    context.Request.Files[i].SaveAs(l_diskPath);

                    //Incluído - Rafael - 03/10/2016 - Se o caminho raiz do arquivo não for o caminho da pasta da Web, ele não tem URL para acesso direto.
                    if (l_rootPath.StartsWith(AppDomain.CurrentDomain.BaseDirectory))
                    {
                        l_WebRelativePath = "/" + l_relativePath.Replace("\\", "/").TrimEnd('/').TrimStart('/');
                        l_AbsoluteUrl = Utils.RelativeToAbsoluteUrl(l_WebRelativePath);
                        l_AppRelativePath = "/" + Utils.ApplicationUrlPath() + l_WebRelativePath;
                    }
                    else
                    {
                        l_WebRelativePath = "";
                        l_AbsoluteUrl = "";
                        l_AppRelativePath = "";
                    }

                    
                    l_newHeight = 0;
                    l_newWidth = 0;
                    if (l_height != 0 && l_width != 0 && (l_FileExtension == ".jpg" || l_FileExtension == ".jpeg" || l_FileExtension == ".gif" || l_FileExtension == ".png" || l_FileExtension == ".bmp"))
                    {
                        Utils.ResizeImage(l_diskPath, l_width, l_height, out l_newWidth, out l_newHeight);
                    }
                    larrReturnFiles += "{  \"url\": \"" + l_AbsoluteUrl +
                                           "\", \"relative_url\" : \"" + l_WebRelativePath + 
                                           "\", \"app_relative_url\" : \"" + l_AppRelativePath +
                                           "\", \"disc_path\" : \"" + l_diskPath.Replace("\\", "\\\\") +
                                           "\", \"height\": \"" + l_newHeight.ToString() +
                                           "\", \"width\":\"" + l_newWidth.ToString() +
                                           "\", \"param_return\":\"" + l_paramReturn +
                                           "\", \"original_filename\":\"" + l_Filename +
                                           "\", \"file_size\":\"" + l_FileSize +
                                       "\"}," ;
                }

                larrReturnFiles = "[" + larrReturnFiles.TrimEnd(',') + "]";
                
				lstrReturn = string.Concat(larrReturnFiles);
			}
			catch (Exception exception2)
			{
                System.Web.Script.Serialization.JavaScriptSerializer l_JS = new System.Web.Script.Serialization.JavaScriptSerializer();
                string l_ErrorJS = l_JS.Serialize(exception2.ToString());
                lstrReturn = l_ErrorJS;
			}
			context.Response.Write(lstrReturn);
		}

        public static string getRelativeFileStoragePath(string a_Filename, string a_ClassName, string a_FilePathParam)
        {
            string l_rootPath = string.Empty;
            string l_relativePath = string.Empty;
            string l_classRelativePath = string.Empty;
            string l_fullClassName = string.Empty;
            string l_fullDiskPath = string.Empty;
            string l_className = a_ClassName;
            string l_param = a_FilePathParam;
            string l_Filename = a_Filename;

            //Alterado - Rafael - 03/10/2016
            if (l_className.IndexOf(".") > -1)
                l_fullClassName = l_className;
            else
                l_fullClassName = string.Concat("ATIMO.Helpers.ModelsUtils.", l_className);

            //Incluído - Rafael - 03/10/2016 - Arquivos do sistema de digitalização ficam em outro lugar
            if (Utils.HasMethod(l_fullClassName, "GetRootDirectory"))
            {
                if(string.IsNullOrEmpty(a_FilePathParam))
                    l_rootPath = Utils.InvokeStringMethod(l_fullClassName, "GetRootDirectory");
                else
                    l_rootPath = Utils.InvokeStringMethod(l_fullClassName, "GetRootDirectory", new string[] { a_FilePathParam });

                l_rootPath = l_rootPath.TrimEnd('\\') + "\\";
            }

            if (string.IsNullOrEmpty(l_rootPath))
            {
                l_rootPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\";
                l_relativePath = (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["FileStorageRelativePath"]) ? ConfigurationManager.AppSettings["FileStorageRelativePath"] : string.Empty);
            }

            if (l_relativePath != string.Empty)
                l_relativePath = l_relativePath.TrimEnd('\\') + "\\";
                        
            if (!Directory.Exists(string.Concat(l_rootPath, l_relativePath)))
            {
                Directory.CreateDirectory(string.Concat(l_rootPath, l_relativePath));
            }
            try
            {
                if (!string.IsNullOrEmpty(l_className))
                {
                    if (!string.IsNullOrEmpty(l_param))
                    {
                        
                        l_classRelativePath = ATIMO.Helpers.Utils.InvokeStringMethod(l_fullClassName, "FileUploadRelativePath", new string[] { l_param });
                    }
                    else
                    {
                        l_classRelativePath = ATIMO.Helpers.Utils.InvokeStringMethod(l_fullClassName, "FileUploadRelativePath");
                    }


                }
            }
            catch (Exception Error)
            {
            }

            if (!string.IsNullOrEmpty(l_classRelativePath))
            {
                l_classRelativePath = l_classRelativePath.TrimStart('\\').TrimEnd('\\') + "\\";
                l_relativePath = l_relativePath + l_classRelativePath;
            }

            if (!Directory.Exists(string.Concat(l_rootPath, l_relativePath)))
            {
                Directory.CreateDirectory(string.Concat(l_rootPath, l_relativePath));
            }

            string l_fileNameConverted = RemoveSpecialCharacters(l_Filename);
            try
            {
                l_fullClassName = string.Concat("ATIMO.Helpers.ModelsUtils.", l_className);

                string l_newFileNameConverted = ATIMO.Helpers.Utils.InvokeStringMethod(l_fullClassName, "FileUploadRenameFile", new string[] { l_fileNameConverted, "" });

                if (!string.IsNullOrEmpty(l_newFileNameConverted))
                    l_fileNameConverted = l_newFileNameConverted;
            }
            catch (Exception exception)
            {
            }
            
            l_relativePath = string.Concat(l_relativePath, l_fileNameConverted);

            return l_relativePath;
        }

        public static string RemoveSpecialCharacters(string a_Filename)
        {
            string l_fileNameConverted = string.Empty;
            string l_Filename = string.Empty;
            if (!string.IsNullOrEmpty(a_Filename))
            {
                l_Filename = a_Filename.Replace(" ", "");
                l_Filename = Utils.RemoveAccents(l_Filename);
                Regex regex = new Regex("(?:[^a-z0-9. ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
                l_fileNameConverted = regex.Replace(l_Filename, string.Empty);
            }
            return l_fileNameConverted;
        }
    }
}