namespace Roham.Data.DbUp
{
    public interface IScriptPreprocessor
    {
        string Process(string contents);
    }
}
