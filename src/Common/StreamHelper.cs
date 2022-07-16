using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Web.UI;

namespace CommonDotNetHelpers.Common
{
    using Microsoft.VisualBasic;
    using System.Reflection;

    public static class StreamHelper
    {

        //    'See also FxLib Author: Kamal Patel, Rick Hodder
        //    'Find the first entry of sToFing and returns the string after it
        //    'See also FxLib StringExtract (and StuffString)
        // Methods
		//public StreamHelper()
		//{
		//}
        #region "Stream and Resources Functions"
        //C# version of FxLib: Extended Functions Library for .NET FXLib
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cFileName"></param>
        /// <returns></returns>
        /// <exception>Throws exception if file not found.See File..::OpenText Method for other exceptions</exception>
        public static string FileToString(string cFileName)
        {
            StreamReader oReader = System.IO.File.OpenText(cFileName);
            string lcString = oReader.ReadToEnd();
            oReader.Close();
            return lcString;
        }
        public static string StreamToString(Stream strm)
        {
            if (strm.CanSeek)
            {
                strm.Seek(0, SeekOrigin.Begin);
            }
            StreamReader reader1 = new StreamReader(strm);
            return reader1.ReadToEnd();
        }
        //'another implementation(similar) in http://blog.steeleprice.net/archive/2004/04/06/202.aspx
        public static string ResourceToString(string ResName)
        {
            Assembly assembly1 = Assembly.GetExecutingAssembly();
            return ResourceToString(ResName, assembly1);
        }
        //                Assembly assembly1 = Assembly.GetExecutingAssembly();
        public static string ResourceByFullNameToString(string ResourceName, Assembly asm)
        {
            Stream stream = asm.GetManifestResourceStream(ResourceName);
            if (stream == null)
            {
                throw new ApplicationException("Couldn't find embedded resource " + ResourceName);
            }
            return StreamToString(stream);
        }
        /// <summary>
		/// Resources are named using a fully qualified name (including namespace).
		///Assume that Default Namespace is the same as Assemebly name
		/// If resource is located in subfolder of C# project, path of subfolder should be specified (it is included as part of namespace)
		/// </summary>
        /// <param name="ResName">case-sensitive</param>
        /// <param name="Asm"></param>
        /// <returns></returns>
        public static string ResourceToString(string ResName, Assembly Asm)
        {
            Stream stream1 = GetManifestResourceStream(ResName, Asm);
            return StreamToString(stream1);
        }
        //          ' Asm - the current caller assembly.
        public static void ResourceToFile(string ResName, Assembly Asm, string FileName)
        {
            //object obj1;
            Stream stream1 = GetManifestResourceStream(ResName, Asm);
            StreamReader reader1 = new StreamReader(stream1);
            StreamWriter writer1 = new StreamWriter(FileName, false);
            writer1.Write(reader1.ReadToEnd());
            writer1.Close();
            //return obj1;
        }
        ///<summary>
        /// Inserts Assembly name in front of ResName, 
		/// If resource is located in subfolder of C# project, path of subfolder should be specified (it is included as part of namespace)
        ///</summary>
        ///<param name="ResName"></param>
        ///<param name="Asm"></param>
        ///<returns></returns>
        public static string ResourceNameWithNamespace(string ResName, Assembly Asm)
        {   //' Resources are named using a fully qualified name (including namespace).
            //assumed that namespace is the same as assembly names
            //TODO use http://www.developersdex.com/gurus/articles/828.asp
            //to find resource even if namespace is not specified
            string sFullName = Asm.GetName().Name + "." + ResName;
            return sFullName;
        }
        //'if ThrowException=true, then Throw exception if resource not found
        public static bool EnsureManifestResourceExist(string ResName, Assembly Asm, bool ThrowException)
        {   //NOTE: If resource is located in subfolder of C# project, path of subfolder should be specified (it is included as part of namespace)
            //' Resources are named using a fully qualified name ((including namespace).
            bool bRet = true;
            ManifestResourceInfo info = Asm.GetManifestResourceInfo(ResName);
            if (info == null)
            {
                bRet = false;
                string sMsg = "Couldn't find embedded resource " + ResName + " in assembly " + Asm.FullName;
                if (ThrowException)
                {
                    throw new ApplicationException(sMsg);
                }
                else
                {
                    Debug.Assert(false, sMsg);
                }
            }
            return bRet;
        }
		
        /// <summary>
		/// If resource is located in subfolder of C# project, path of subfolder should be specified separated by '.'(not '/') (it is included as part of namespace)
		/// Resources are named using a fully qualified name (including namespace).
		/// Assume that Default Namespace is the same as Assemebly name
		/// 'Throw exception if resource not found
        /// </summary>
        /// <param name="ResName"></param>
        /// <param name="Asm"></param>
        /// <returns></returns>
        public static Stream GetManifestResourceStream(string ResName, Assembly Asm)
        {   //NOTE: If resource is located in subfolder of C# project, path of subfolder should be specified (it is included as part of namespace)
            //' Resources are named using a fully qualified name (including namespace).
            //Assume that Default Namespace is the same as Assemebly name
            string sName = Asm.GetName().Name + "." + ResName;
            Stream stream2 = Asm.GetManifestResourceStream(sName);
            if (stream2 == null)//according to ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref10/html/M_System_Reflection_Assembly_GetManifestResourceStream_2_42a1d385.htm 
            {// .Net 2 throws exception by itself
                throw new ApplicationException("Couldn't find embedded resource " + sName);
            }
            return stream2;
        }
		//
        /// <summary>
        /// Finds resource from Calling Assembly
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks><seealso cref="StreamToString"/></remarks>
        public static Stream FindResourceStream(string name)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            return FindResourceStream(name, assembly);
        }
	
        public static Stream FindResourceStream(string name, Assembly assembly)
        {
            string resName = FindResourceFullName(name, assembly);
            Stream stream = assembly.GetManifestResourceStream(resName);
            return stream;
        }
		public static string FindResourceAsString(string name, Assembly assembly)
		{
			Stream stream = FindResourceStream(name, assembly);
			return StreamToString(stream);
		}
        //moved from CommonTestUtilities
        public static string FindResourceFullName(string resName)
        {
            return FindResourceFullName(resName, Assembly.GetCallingAssembly());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string FindResourceFullName(string resName, Assembly assembly)
        {
            string[] names = assembly.GetManifestResourceNames();
            foreach (string str in names)
            {   
                if (str.EndsWith(resName, StringComparison.OrdinalIgnoreCase))
                {
                    return str;
                }
            }
            string message =String.Format("Could not find resource {0} in unit test assembly {1} " , resName,assembly.FullName);
            throw new ApplicationException(message);
        }


        public static void SaveToFile(byte[] data, string fileName)
        {
            StreamWriter writer1 = new StreamWriter(fileName, false);
            writer1.BaseStream.Write(data, 0, data.Length);
            writer1.Close();
        }
        public static void SaveToFile(Stream strm, string fileName)
        {
            StreamReader reader = new StreamReader(strm);
            StreamWriter writer = new StreamWriter(fileName, false);
            writer.WriteLine(reader.ReadToEnd());
            writer.Close();
        }
        public static void SaveStringToFile(string sData, string fileName)
        {
            StreamWriter writer1 = new StreamWriter(fileName, false);
            writer1.Write(sData);
            writer1.Close();
        }
        //From http://stackoverflow.com/questions/230128/best-way-to-copy-between-two-stream-instances-c
		// there's a Stream.CopyTo method:   input.CopyTo(output);

		//public static Stream Copy(Stream fromStream, Stream toStream)
		//{
		//    StreamReader reader = new StreamReader(fromStream);
		//    StreamWriter writer = new StreamWriter(toStream);
		//    writer.WriteLine(reader.ReadToEnd());
		//    writer.Flush();
		//    return toStream;
		//}

        #endregion //"Stream and Resources Functions"

		
 
		/// <summary>
		/// from http://geekswithblogs.net/mnf/archive/2006/05/26.aspx
		/// 		call the function in Debug mode only,e.g.
		///  Debug.Assert(StreamHelper.EnsureWebResourceValid(sFullName, rsType.Assembly,true));
		/// </summary>
		/// <param name="ResName"></param>
		/// <param name="Asm"></param>
		/// <param name="ThrowException"></param>
		/// <returns></returns>
        public static bool EnsureWebResourceValid(string ResName, Assembly Asm,bool ThrowException)
        {
			bool bRet = StreamHelper.EnsureManifestResourceExist(ResName, Asm, ThrowException);
 
            if(bRet==true)
 
            { //find the attribute
 
                bRet = false;
 
                // Iterate through the attributes for the assembly.
 
                foreach (Attribute attr in Attribute.GetCustomAttributes(Asm))

                {
                	//Check for WebResource attributes.

                	var wra = attr as WebResourceAttribute;
                	if (wra != null)
                	{
                		Debug.WriteLine ("Resource in the assembly: " + wra.WebResource.ToString() +
 
                		                 " with ContentType = " + wra.ContentType.ToString() +
 
                		                 " and PerformsSubstitution = " + wra.PerformSubstitution.ToString() );
 
                		if (wra.WebResource== ResName)
 
                		{
 
                			bRet = true;
 
                			break;
 
                		}
 
                	}
                } //foreach
 
            } //ManifestResourceExist
 
            if(bRet==false) 

            {
 
                string sMsg="Embedded resource " + ResName + " in assembly " + Asm.FullName + " doesn't have "WebResourceAttribute ";
 
                if (ThrowException == true)
 
                {
 
                   throw new ApplicationException(sMsg);
 
                }
 
                else
 
                {
 
                    Debug.Assert(false, sMsg);
 
                }
 
            }
 
            return bRet;
 
        }
 
   

    }
}
