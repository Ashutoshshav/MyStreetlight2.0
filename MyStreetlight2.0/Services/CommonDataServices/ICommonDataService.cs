namespace Streetlight2._0.Services.CommonDataService
{
    public interface ICommonDataService
    {
        Task<string> GetLightStsNameByStsId(int stsId);
    }
}
