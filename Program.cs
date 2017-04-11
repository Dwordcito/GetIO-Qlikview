using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetIO
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string file in Directory.EnumerateFiles(@"C:\Users\e36125732\Desktop\SCRIPTSCARGA\", "*.txt"))
            {
                //OUTPUT
                string contents = File.ReadAllText(file);
                List<String> lineas;
                contents = eliminarComentarios(contents);
                lineas = traerLineasStore(contents);
                lineas = eliminarParametros(lineas, 1);
                grabarArchivo(lineas, file.Substring(file.LastIndexOf(@"\") + 1), 1);

                //INPUT
                contents = File.ReadAllText(file);
                contents = eliminarComentarios(contents);
                lineas = traerLineasFrom(contents);
                lineas = eliminarParametros(lineas, 2);
                grabarArchivo(lineas, file.Substring(file.LastIndexOf(@"\") + 1), 2);

            }
        }
        static List<String> traerLineasFrom(String contents)
        {
            List<String> lineastemp = new List<String>();
            List<String> lineas = new List<String>();
            int last, first;
            while ((first = contents.ToLower().IndexOf("load")) != -1)
            {
                last = contents.ToLower().IndexOf(";", first);
                lineastemp.Add(contents.Substring(first, last - first+1));
                contents = contents.Remove(first, last - first);

            }
            for (int i = 0; i < lineastemp.Count; i++)
            {
                lineastemp[i] = lineastemp[i].Replace('\t', ' ');
                lineastemp[i] = lineastemp[i].Replace('\r', ' ');
                lineastemp[i] = lineastemp[i].Replace('\n', ' ');
                first = lineastemp[i].ToLower().IndexOf(" from ");
                if (first < 1) continue;
                last = lineastemp[i].ToLower().IndexOf(";", first);
                if (last < 1) continue;
                lineas.Add(lineastemp[i].Substring(first, last - first + 1));
            }
            return lineas;
        }
       

        static void grabarArchivo(List<String> lineas, String file, int mode)
        {
            String archivo;
            int counter = 0;
            if (mode == 1)
            {
                archivo = "output";
            }else if(mode == 2)
            {
                archivo = "input";
            } else
            {
                return;
            }

            if (!File.Exists(archivo))
            {
                using (StreamWriter sw = File.CreateText(archivo))
                {
                    if (lineas.Count > 0)
                    {
                        sw.WriteLine(lineas[0] + ";" + file);
                    }
                }
                counter++;
            }

            for (int i = 0 + counter; i < lineas.Count; i++)
            {
                using (StreamWriter sw = File.AppendText(archivo))
                {
                    sw.WriteLine(lineas[i] + ";" + file);
                }
            }
        }

        static List<String> eliminarParametros(List<String> lineas, int mode)
        {
            int last, first;
            List<String> texto = new List<String>();

            String tipo;
            if (mode == 1)
            {
                tipo = " into ";
                texto.Add("(qvd");
                texto.Add("(txt");
                texto.Add("(ansi");
                texto.Add("( txt");
            }
            else if (mode == 2)
            {
                tipo = " from ";
                texto.Add("(biff");
                texto.Add("(ooxml");
                texto.Add("(qvd");
                texto.Add("(txt");
                texto.Add("(xml");
                texto.Add("(html");
                texto.Add("(fix");
                texto.Add("(utf8");
            }
            else
            {
                return null;
            }

            for (int i = 0; i < lineas.Count(); i++)
            {
                while (lineas[i].IndexOf("]") != -1)
                {
                    lineas[i] = lineas[i].Remove(lineas[i].IndexOf("]"), 1);
                }
                while (lineas[i].IndexOf("[") != -1)
                {
                    lineas[i] = lineas[i].Remove(lineas[i].IndexOf("["), 1);
                }

                while (lineas[i].IndexOf("\t") != -1)
                {
                    //lineas[i] = lineas[i].Remove(lineas[i].IndexOf("\t"), 1);
                    lineas[i] = lineas[i].Replace('\t', ' ');
                }

                while (lineas[i].IndexOf("\r") != -1)
                {
                    //lineas[i] = lineas[i].Remove(lineas[i].IndexOf("\t"), 1);
                    lineas[i] = lineas[i].Replace('\r', ' ');
                }

                while (lineas[i].IndexOf("\n") != -1)
                {
                    //lineas[i] = lineas[i].Remove(lineas[i].IndexOf("\t"), 1);
                    lineas[i] = lineas[i].Replace('\n', ' ');
                }


                while ((first = lineas[i].ToLower().IndexOf(tipo)) != -1)
                {
                    for (int j = 0; j < texto.Count; j++)
                    {
                        last = lineas[i].ToLower().IndexOf(texto[j], first);
                        if (last > 0)
                        {
                            lineas[i] = lineas[i].Substring(first + tipo.Count(), last - first - tipo.Count());
                            break;
                        }
                        if (texto.Count-1 == j)
                        {
                            while (lineas[i].ToLower().IndexOf("group by") != -1)
                            {
                                lineas[i] = lineas[i].Remove(lineas[i].ToLower().IndexOf("group by"), (lineas[i].Length - lineas[i].ToLower().IndexOf("group by")));
                            }
                            while (lineas[i].ToLower().IndexOf("order by") != -1)
                            {
                                lineas[i] = lineas[i].Remove(lineas[i].ToLower().IndexOf("order by"), (lineas[i].Length - lineas[i].ToLower().IndexOf("order by")));
                            }
                            while (lineas[i].ToLower().IndexOf("where") != -1)
                            {
                                lineas[i] = lineas[i].Remove(lineas[i].ToLower().IndexOf("where"), (lineas[i].Length - lineas[i].ToLower().IndexOf("where")));
                            }
                            lineas[i] = lineas[i].ToLower().Replace(" from ", "");
                            while (lineas[i].ToLower().IndexOf(" into ") != -1)
                            {
                                lineas[i] = lineas[i].Remove(0, lineas[i].ToLower().IndexOf(" into ")+6);
                            }
                            break;
                        }
                    }
                    
                }
                lineas[i] = lineas[i].Trim();
            }
            return lineas;
        }

        static List<String> traerLineasStore(String contents)
        {
            List<String> lineas = new List<String>();
            int last, first;
            while ((first = contents.ToLower().IndexOf("store")) != -1)
            {
                last = contents.ToLower().IndexOf(";", first);
                if (last > 0)
                {
                    lineas.Add(contents.Substring(first + 5, last - first - 5));
                    contents = contents.Remove(first, last - first);
                }
            }
            return lineas;
        }

        static String eliminarComentarios(String contents)
        {
            int first = 0;
            int last = 0;
            while ((first = contents.ToLower().IndexOf("//")) >= 0)
            {
                last = contents.ToLower().IndexOf("\r\n", first);
                if (last == -1) { last = contents.Length; }
                contents = contents.Remove(first, last - first);
            }
           
            first = 0;
            last = 0;
            while ((first = contents.ToLower().IndexOf("/*")) != -1)
            {
                last = contents.ToLower().IndexOf("*/", first);
                if (last == -1) last = contents.Length;
                contents = contents.Remove(first, last - first);
            }
            return contents;
        }
    }
}
