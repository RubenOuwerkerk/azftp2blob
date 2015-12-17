using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.General;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// base class for RMD & XRMD command handlers
    /// </summary>
    internal class RemoveDirectoryCommandHandlerBase : FtpCommandHandler
    {
        protected RemoveDirectoryCommandHandlerBase(string sCommand, FtpConnectionObject connectionObject)
            : base(sCommand, connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();
            if (sMessage == "")
                return GetMessage(501, string.Format("{0} needs a parameter", Command));

            string dirToRemove = GetPath(FileNameHelpers.AppendDirTag(sMessage));

            // check whether directory exists
            if (!ConnectionObject.FileSystemObject.DirectoryExists(dirToRemove))
            {
                FtpServer.LogWrite(this, sMessage, 550, 0);
                return GetMessage(550, string.Format("Directory \"{0}\" does not exist", dirToRemove));
            }

            // can not delete root directory
            if (dirToRemove == "/")
            {
                FtpServer.LogWrite(this, sMessage, 553, 0);
                return GetMessage(553, "Can not remove root directory");
            }

            // delete directory
            if (ConnectionObject.FileSystemObject.DeleteDirectory(dirToRemove))
            {
                FtpServer.LogWrite(this, sMessage, 250, 0);
                return GetMessage(250, string.Format("{0} successful.", Command));
            }
            else
            {
                FtpServer.LogWrite(this, sMessage, 550, 0);
                return GetMessage(550, string.Format("Couldn't remove directory ({0}).", dirToRemove));
            }
        }
    }
}