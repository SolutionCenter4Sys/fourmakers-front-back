using DotNetEnv;

namespace Colaboracao.Helper
{
    public static class VariaveisDeAmbienteUtil
    {
        public static string GetVariavelDeAmbiente(string variavel)
        {
#if (DEBUG)
            Env.TraversePath().Load();
            return Env.GetString(variavel);
#else
             return System.Environment.GetEnvironmentVariable(variavel);
#endif
        }
    }
}