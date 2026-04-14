namespace DataTransferObject.Domain.Util;

public class PdfSignatureLine
{
    public string Text { get; set; }
    public int PosY { get; set; } 
    public int PosX { get; set; }
    public int FontSize { get; set; }
}