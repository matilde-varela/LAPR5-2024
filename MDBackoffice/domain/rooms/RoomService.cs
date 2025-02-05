using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDBackoffice.Domain.RoomTypes;
using MDBackoffice.Domain.Shared;
using Microsoft.Extensions.ObjectPool;

namespace MDBackoffice.Domain.Rooms
{
    public class RoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomRepository _repo;
        private readonly IRoomTypeRepository _repoType;

        public RoomService(IUnitOfWork unitOfWork, IRoomRepository repo, IRoomTypeRepository typeRepo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _repoType = typeRepo;
        }

        public virtual async Task<RoomDto> AddAsync(CreateRoomDto dto)
        {
            var room = await _repo.GetByIdAsync(new RoomNumber(dto.RoomNumber));

            if (room != null)
            {
               throw new InvalidOperationException("A room with that number already exists.");
            }

            var roomType = await _repoType.GetByDesignationAsync(dto.TypeDesignation);

            var availableEquipmentList = new List<Equipment>();

            var slotList = new List<Slot>();

            if(dto.AvailableEquipment != null)
            {
                for(int i = 0; i < dto.AvailableEquipment.Count; i++)
                {
                    availableEquipmentList.Add(new Equipment(dto.AvailableEquipment.ElementAt(i)));
                }
            }

            if(dto.MaintenanceSlots != null)
            {
                for(int i = 0; i < dto.MaintenanceSlots.Count; i++)
                {
                    var slot = dto.MaintenanceSlots.ElementAt(i);
                    slot = slot.Replace("[","");
                    slot = slot.Replace("]","");
                    var slotTimes = slot.Split("-");

                    slotList.Add(new Slot(slotTimes.ElementAt(0), slotTimes.ElementAt(1), DateTime.Now.ToString("yyyy-MM-dd"), null));
                }
            }

            room = new Room(
                new RoomNumber(dto.RoomNumber),
                roomType.Id,
                new Capacity(dto.Capacity),
                availableEquipmentList,
                CurrentStatus.Available,
                slotList
            );

            await _repo.AddAsync(room);
            await _unitOfWork.CommitAsync();

            return room.ToDto();
        }

        public async Task<List<RoomDto>> GetAllAsync()
        {
            var list = await _repo.GetAllRoomsAsync();

            List<RoomDto> listDto = list.ConvertAll(room =>
               new RoomDto
               (room.Id.AsString(), room.Type.Value, room.Capacity.CapcityNumber,
               room.AvailableEquipment.Select(equipment => equipment.EquipmentName).ToList(), room.CurrentStatus.Description,
                   room.MaintenanceSlots.Select(slot => new SlotsDto(slot.Date.Start.ToString(), slot.Date.End.ToString(),
                   slot.TimeInterval.Start.ToString(), slot.TimeInterval.End.ToString())).ToList()
               ));

            return listDto;
        }

        public async Task<RoomDto> GetRoomDtoById(string id)
        {
            var room = await _repo.GetByIdAsync(new RoomNumber(id));
            RoomDto roomDto = room.ToDto();
            return roomDto;
        }

        public async Task<Room> GetRoomById(string id)
        {
            var room = await _repo.GetByIdAsync(new RoomNumber(id));
            return room;
        }

        public async Task<List<RoomScheduleDto>> GetAllSchedules()
        {
            var rooms = await _repo.GetAllRoomsAsync();

            var listDto = rooms.Select(room =>
                new RoomScheduleDto(
                    room.Id.AsString(),
                    room.MaintenanceSlots.Select(slot => new SlotsDto
                    {
                        StartDate = slot.Date.Start.ToString(),
                        EndDate = slot.Date.End.ToString(),
                        StartTime = slot.TimeInterval.Start.ToString(),
                        EndTime = slot.TimeInterval.End.ToString(),
                        Name = slot.Description
                    }).ToList()
                )
            ).ToList();

            return listDto;
        }

        public async Task<List<string>> GetAllRoomNumbersAsync()
        {
            var list = await _repo.GetAllRoomsAsync();

            List<string> listDto = list.ConvertAll(room =>
               new string (room.Id.Value));

            return listDto;
        }
    }
}