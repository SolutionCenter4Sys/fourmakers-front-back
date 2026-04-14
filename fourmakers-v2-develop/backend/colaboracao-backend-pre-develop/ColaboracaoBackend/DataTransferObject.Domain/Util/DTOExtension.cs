namespace DataTransferObject.Domain.Util
{
    public static class DTOExtension
    {
        public static void AtualizarSafeComPropriedadesDe<T>(this T destino, T origem) where T : class
        {
            if (destino != null && origem != null)
            {
                foreach (var property in typeof(T).GetProperties())
                {
                    // ignorar propriedades somente de leitura (sem setter) ou com valores nulos
                    if (property.CanWrite)
                    {
                        var value = property.GetValue(origem);
                        property.SetValue(destino, value);
                    }
                }
            }
        }
    }
}