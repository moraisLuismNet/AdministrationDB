namespace AdministrationDataBase.Helpers
{
    public class TemplateHelper
    {
        public static string GetTemplateString(string path, string stylePath, IWebHostEnvironment _env, string[] data = null)
        {
            try
            {
                path = _env.WebRootPath

                                + Path.DirectorySeparatorChar.ToString()

                                + "EmailTemplates"

                                + Path.DirectorySeparatorChar.ToString()

                               + path;

                stylePath = _env.WebRootPath

                                + Path.DirectorySeparatorChar.ToString()

                                + "EmailTemplates"

                                + Path.DirectorySeparatorChar.ToString()

                               + stylePath;


                string templateStr = "";

                using (StreamReader reader = File.OpenText(path))
                {
                    templateStr = reader.ReadToEnd();
                }

                if (data != null)
                {
                    templateStr = String.Format(templateStr, data);
                }

                using (StreamReader reader = File.OpenText(stylePath))
                {
                    var styleString = reader.ReadToEnd();
                    templateStr = templateStr.Replace("[style]", styleString);
                }

                return templateStr;
            }
            catch
            {
                return "";
            }
        }
    }
}
