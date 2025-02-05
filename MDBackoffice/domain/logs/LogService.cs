using System;
using System.Threading.Tasks;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Logs
{
    public class LogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogRepository _repo;

        public LogService(IUnitOfWork unitOfWork, ILogRepository repo)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
        }
        
        public virtual async Task<bool> CreateDeletionLog(string objectReference, string objectClass, string description)
        {
            try
            {
                Log log = new Log(await getSequentialNumber(), objectClass, objectReference, 1, description);

                await _repo.AddAsync(log);
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public virtual async Task<bool> CreateCreationLog(string objectReference, string objectClass, string description)
        {
            try
            {
                Log log = new Log(await getSequentialNumber(), objectClass, objectReference, 2, description);
                
                await _repo.AddAsync(log);
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateEditLog(string objectReference, string objectClass, string description)
        {
            try
            {
                Log log = new Log(await getSequentialNumber(), objectClass, objectReference, 3, description);
                
                await _repo.AddAsync(log);
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<string> getSequentialNumber()
        {
            try
            {
                LogId logId = await _repo.FindLastLogIdAsync();
                string lastSequentialNumber = logId.AsString().Substring(4);
                int newSequentialNumber = int.Parse(lastSequentialNumber) + 1;
                return newSequentialNumber.ToString("D5");
            }
            catch (NullReferenceException)
            {
                return "00001";
            }
        }
    }
}
