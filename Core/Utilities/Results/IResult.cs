namespace Core.Utilities.Results
{
    //Temel voidler için başlangıç
    //işlem sonucu ve gösterilcek mesaj
    public interface IResult
    {
        bool Success { get; }
        string Message { get; }
    }
}