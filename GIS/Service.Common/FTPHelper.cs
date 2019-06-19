using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
namespace Service.Common
{
    /// <summary>
    /// FTP帮助类
    /// </summary>
    public class FTPHelper
    {
        #region 变量
        public string ftpURI;
        public string ftpUserID;
        public string ftpServerUrl;
        public string ftpPassword;
        public string ftpRemotePath;
        public string strEncodingType;
        #endregion

        /// <summary>  
        /// 连接FTP服务器
        /// </summary>
        /// <param name="FtpServerIP">FTP连接地址</param>
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        /// <param name="FtpUserID">用户名</param>
        /// <param name="FtpPassword">密码</param>
        /// <param name="StrEncodingType">编码格式</param>
        public FTPHelper(string FtpServerurl, string FtpRemotePath, string FtpUserID, string FtpPassword, string StrEncodingType)
        {
            ftpServerUrl = FtpServerurl;
            ftpRemotePath = FtpRemotePath;
            ftpUserID = FtpUserID;
            ftpPassword = FtpPassword;
            strEncodingType = StrEncodingType;
            ftpURI = ftpServerUrl;
            if (!string.IsNullOrEmpty(ftpRemotePath))
            {
                ftpURI = ftpServerUrl + ftpRemotePath + "/";
            }
        }

        /// <summary>  
        /// 上传  
        /// </summary>   
        public void Upload(string filename)
        {
            FileInfo fileInf = new FileInfo(filename);
            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + fileInf.Name));
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.KeepAlive = false;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            FileStream fs = fileInf.OpenRead();
            try
            {
                Stream strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Upload(string filename, bool isdelete)
        {
            FileInfo fileInf = new FileInfo(filename);
            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + fileInf.Name));
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.KeepAlive = false;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            FileStream fs = fileInf.OpenRead();
            try
            {
                Stream strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                strm.Close();
                fs.Close();
                if (isdelete)
                {
                    File.Delete(filename);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>  
        /// 下载  
        /// </summary>   
        public void Download(string filePath, string fileName)
        {
            try
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                FileStream outputStream = new FileStream(filePath + "\\" + fileName, FileMode.Create);
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + fileName));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = true;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>  
        /// 删除文件  
        /// </summary>  
        public void Delete(string fileName)
        {
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + fileName));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.KeepAlive = false;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = true;
                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>  
        /// 删除目录  
        /// </summary>  
        public void DirDelete(string fileName)
        {
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + fileName));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                reqFTP.KeepAlive = false;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = true;
                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>  
        /// 获取当前目录下明细(包含文件和文件夹)  
        /// </summary>  
        public string[] GetFilesDetailList()
        {
            try
            {
                StringBuilder result = new StringBuilder();
                FtpWebRequest ftp;
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftp.UseBinary = true;
                ftp.UsePassive = true;
                WebResponse response = ftp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), GetCurrentEncoding());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                if (result.ToString().IndexOf("\n") > 0)
                {
                    result.Remove(result.ToString().LastIndexOf("\n"), 1);
                }
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 设置编码格式
        /// </summary>
        /// <returns></returns>
        private Encoding GetCurrentEncoding()
        {
            Encoding mEncoding = Encoding.UTF8;
            string text = strEncodingType;
            if (text != null)
            {
                if (!(text == "UTF8"))
                {
                    if (!(text == "Default"))
                    {
                        if (text == "Unicode")
                        {
                            mEncoding = Encoding.Unicode;
                        }
                    }
                    else
                    {
                        mEncoding = Encoding.Default;
                    }
                }
                else
                {
                    mEncoding = Encoding.UTF8;
                }
            }
            else
            {
                mEncoding = Encoding.Default;
            }
            return mEncoding;
        }

        /// <summary>  
        /// 获取FTP文件列表(包括文件夹)
        /// </summary>   
        private string[] GetAllList(string url)
        {
            List<string> list = new List<string>();
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(new Uri(url));
            req.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            req.Method = WebRequestMethods.Ftp.ListDirectory;
            req.UseBinary = true;
            req.UsePassive = true;
            try
            {
                using (FtpWebResponse res = (FtpWebResponse)req.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(res.GetResponseStream(), GetCurrentEncoding()))
                    {
                        string s;
                        while ((s = sr.ReadLine()) != null)
                        {
                            list.Add(s);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return list.ToArray();
        }

        /// <summary>  
        /// 获取当前目录下文件列表(不包括文件夹)  
        /// </summary>  
        public string[] GetFileList()
        {
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = true;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), GetCurrentEncoding());
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Length > 54)//去掉前54字符
                    {
                        string name = line.Remove(0, 54).Trim();
                        if (!(name == "." || name == ".."))
                        {
                            result.Append(name);
                            result.Append("\n");
                        }
                    }
                    line = reader.ReadLine();
                }
                if (!string.IsNullOrEmpty(result.ToString()))
                {
                    result.Remove(result.ToString().LastIndexOf('\n'), 1);
                }
                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            if (!string.IsNullOrEmpty(result.ToString()))
            {
                return result.ToString().Split('\n');
            }
            else
            {
                return null;
            }
        }



        /// <summary>  
        /// 获取当前目录下文件列表(不包括文件夹)  
        /// </summary>  
        public string[] GetFileListNew()
        {
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.KeepAlive = false;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = true;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), GetCurrentEncoding());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return result.ToString().Split('\n');
        }

        /// <summary>  
        /// 判断当前目录下指定的文件是否存在  
        /// </summary>  
        /// <param name="RemoteFileName">远程文件名</param>  
        public bool FileExist(string RemoteFileName)
        {
            string[] fileList = GetFileList();
            foreach (string str in fileList)
            {
                if (str.Trim() == RemoteFileName.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>  
        /// 获取当前目录下文件夹列表(不包括文件)  
        /// </summary>  
        public List<string> GetDirList()
        {
            List<string> result = new List<string>();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = true;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), GetCurrentEncoding());
                string line = reader.ReadLine();
                while (line != null)
                {
                    //此方法获取不到文件夹名称
                    //if (line.IndexOf("<DIR>") > 0)
                    //{
                    //    result.Add(line);
                    //}
                    if (line.Length > 54)//去掉前54字符
                    {
                        result.Add(line.Remove(0, 54).Trim());
                    }
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return result;
        }

        /// <summary>  
        /// 判断当前目录下指定的文件夹是否存在  
        /// </summary>  
        /// <param name="DirName">远程文件夹名</param>  
        public bool DirExist(string DirName)
        {
            List<string> dirList = GetDirList();
            foreach (string dir in dirList)
            {
                if (dir.Contains(DirName))
                {
                    return true;
                }
                //if (dir.Remove(29).Contains("<DIR>"))
                //{
                //    string temp = dir.Remove(0, 39);
                //    if (temp == DirName.Trim())
                //    {
                //        return true;
                //    }
                //}
            }
            return false;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        public void MakeDir(string dirName)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + dirName));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                //指定执行什么命令 
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                //指定数据传输类型
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = true;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        /// <summary>  
        /// 获取指定文件大小  
        /// </summary>  
        public long GetFileSize(string filename)
        {
            FtpWebRequest reqFTP;
            long fileSize = 0;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + filename));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = true;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                fileSize = response.ContentLength;
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return fileSize;
        }

        /// <summary>  
        /// 更改文件名  
        /// </summary> 
        public void ReName(string currentFilename, string newFilename)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + currentFilename));
                reqFTP.RenameTo = newFilename;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = true;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        /// <summary>  
        /// 移动文件  
        /// </summary>  
        public void MovieFile(string currentFilename, string newDirectory)
        {
            ReName(currentFilename, newDirectory);
        }

        /// <summary>  
        /// 切换当前目录  
        /// </summary>  
        /// <param name="IsRoot">true:绝对路径 false:相对路径</param>   
        public void GotoDirectory(string DirectoryName, bool IsRoot)
        {
            if (IsRoot)
            {
                ftpRemotePath = DirectoryName;
            }
            else
            {
                if (!string.IsNullOrEmpty(ftpRemotePath))
                {
                    ftpRemotePath = ftpRemotePath + "/";
                }
                ftpRemotePath += DirectoryName + "/";
            }
            ftpURI = ftpServerUrl;
            if (!string.IsNullOrEmpty(ftpRemotePath))
            {
                ftpURI = ftpServerUrl + ftpRemotePath + "/";
            }
        }
    }
}