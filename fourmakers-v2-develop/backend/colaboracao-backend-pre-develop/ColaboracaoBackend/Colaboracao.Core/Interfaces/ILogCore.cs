using DataTransferObject.Domain.Log;

namespace Colaboracao.Core
{
    public interface ILogCore
    {
        void Log(string message, LevelsEnum level);
    }
}