using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;

namespace MDBackoffice.Infrastructure.OperationRequests
{
    public class PlanningSchedulerAdapter : IOperationSchedulerAdapter
    {
        public async Task<List<SchedulesDto>> ScheduleOperationsAsync(
            Dictionary<ScheduleOperationRequestDto, List<ScheduleStaffDto>> operationsMap,
            List<RoomDto> room,
            string day,
            string algorithm)
        {

            string json = CreateScheduleJson(operationsMap, room, day);
            Console.WriteLine(json);

            string url = string.Empty; // Declare the url variable before the if-else block

            if (algorithm.Equals("better-solution"))
            {
                url = "http://localhost:8080/api/p/better-solution";
            }
            else if (algorithm.Equals("first-doctor"))
            {
                url = "http://localhost:8080/api/p/heuristic-first-available";
            }
            else if (algorithm.Equals("highest-occupancy"))
            {
                url = "http://localhost:8080/api/p/heuristic-highest-occupancy";
            }
            else if (algorithm.Equals("genetic-room-distribution"))
            {
                url = "http://localhost:8080/api/p/room-distribution-and-genetic";
            }

            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                try
                {
                    var response = httpClient.PostAsync(url, content).Result;
                    response.EnsureSuccessStatusCode();
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(responseBody);

                    if (algorithm.Equals("genetic-room-distribution"))
                    {
                        return ConvertJsonGeneticToDto(responseBody);
                    }
                    else
                    {
                        return ConvertJsonToDto(responseBody);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                    throw;
                }
            }
        }


        private string CreateScheduleJson(Dictionary<ScheduleOperationRequestDto, List<ScheduleStaffDto>> operationsMap,
            List<RoomDto> room,
            string day)
        {
            var agRoom = room.Select(r => new
            {
                roomId = r.RoomNumber,
                date = day.Replace("-", ""),
                occupied = r.MaintenanceSlots.Select(slot => new
                {
                    start = ConvertToMinutes(slot.StartTime),
                    end = ConvertToMinutes(slot.EndTime),
                    operationId = slot.Name ?? "occupied"
                }).ToList()
            }).ToList();


            var staff = operationsMap
                .SelectMany(op => op.Value, (operation, staff) => new
                {
                    staffId = staff.Id,
                    function = staff.Function.ToLower(),
                    spec = staff.SpecializationId.ToLower(),
                    assocOp = new List<string>()
                })
                .Distinct() // Remove duplicates if staff are referenced in multiple operations
                .ToList();

            var agStaff = operationsMap
                .SelectMany(op => op.Value, (operation, staff) => new
                {
                    staffId = staff.Id,
                    date = int.Parse(day.Replace("-", "")),
                    schedule = staff.Slots.Select(slot => new
                    {
                        start = ConvertToMinutes(slot.StartTime),
                        end = ConvertToMinutes(slot.EndTime),
                        operationId = slot.Name
                    }).ToList()
                })
                .Distinct() // Remove duplicates if staff are referenced in multiple operations
                .ToList();

            var timetable = operationsMap
               .SelectMany(op => op.Value, (operation, staff) => new
               {
                   staffId = staff.Id,
                   date = int.Parse(day.Replace("-", "")),
                   availability = new
                   {
                       start = 400, // Combine date and time in ISO format
                       end = 1400,
                   }
               })
               .Distinct() // Remove duplicates if staff are referenced in multiple operations
               .ToList();

            var surgery = operationsMap
                .Keys // Get all OperationRequestDto keys from the dictionary
                .Select(operation => new
                {
                    surgeryId = operation.OperationTypeId,
                    prepTime = operation.PrepTime,
                    surgeryDuration = operation.SurgTime,
                    recoveryTime = operation.CleanTime
                })
                .ToList();

            var surgeryId = operationsMap
              .Keys // Get all OperationRequestDto keys from the dictionary
              .Select(operation => new
              {
                  operationId = operation.Id,
                  surgeryId = operation.OperationTypeId
              })
              .ToList();


            var assignSurgery = operationsMap
                .SelectMany(op => op.Value, (operation, staff) => new
                {
                    operationId = operation.Key.Id,
                    staffId = staff.Id
                })
                .ToList();
            var allData = new
            {
                agRoom,
                staff,
                surgery,
                agStaff,
                timetable,
                surgeryId,
                assignSurgery
            };

            return JsonSerializer.Serialize(allData, new JsonSerializerOptions { WriteIndented = true });
        }

        private int ConvertToMinutes(string time)
        {
            if (string.IsNullOrWhiteSpace(time))
                throw new ArgumentException("Time string cannot be null or empty.");

            // Split the time string into hours and minutes
            string[] timeParts = time.Split(':');
            if (timeParts.Length != 2 && timeParts.Length != 3)
                throw new FormatException("Invalid time format. Expected format: 'HH:mm' or 'HH:mm:ss'.");

            // Parse the hours and minutes
            if (!int.TryParse(timeParts[0], out int hours) || !int.TryParse(timeParts[1], out int minutes))
                throw new FormatException("Invalid numeric values in time string.");

            if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59)
                throw new ArgumentOutOfRangeException("Hours or minutes are out of valid range.");

            // Calculate total minutes
            return (hours * 60) + minutes;
        }

        public string ConvertToTime(int totalMinutes)
        {
            // Calculate hours and minutes from total minutes
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;

            // Return the formatted string as "HH:mm"
            return $"{hours:D2}:{minutes:D2}:00";
        }


        private List<SchedulesDto> ConvertJsonGeneticToDto(string json)
        {
            var parsedJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            // Initialize the list to store the results
            List<SchedulesDto> schedules = new List<SchedulesDto>();

            // Check if "replies" exists and is an array
            if (parsedJson.ContainsKey("replies") && parsedJson["replies"].ValueKind == JsonValueKind.Array)
            {
                foreach (var reply in parsedJson["replies"].EnumerateArray())
                {
                    // Check if RoomSchedule and StaffSchedule are not null
                    if (reply.TryGetProperty("RoomSchedule", out var roomSchedule) && roomSchedule.ValueKind != JsonValueKind.Null &&
                        reply.TryGetProperty("StaffSchedule", out var staffSchedule) && staffSchedule.ValueKind != JsonValueKind.Null)
                    {
                        // Call ConvertJsonToDto and combine both schedules
                        var schedulesForReply = ConvertJsonToDto(reply.ToString());
                        schedules.AddRange(schedulesForReply);
                    }
                }
            }

            return schedules;
        }


        private List<SchedulesDto> ConvertJsonToDto(string json)
        {
            // Parse the JSON string into a dictionary
            var parsedJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            // Extract the fields from the parsed JSON
            string date = parsedJson["Day"].GetInt32().ToString(); // This is in "yyyyMMdd" format
            string roomId = parsedJson["Room"].GetString();

            // Parse the date from the "Day" field
            var startDate = DateTime.ParseExact(date, "yyyyMMdd", null);

            // Helper function to convert timestamps to SlotsDto
            Func<JsonElement, SlotsDto> toSlotsDto = slot =>
            {
                int start = slot.GetProperty("start").GetInt32();
                int end = slot.GetProperty("end").GetInt32();
                string operationId = slot.TryGetProperty("operationId", out var id) ? id.GetString() : null;

                // Use the Day field directly as StartDate and EndDate
                return new SlotsDto(
                    startDate.ToString("yyyy-MM-dd"), // StartDate
                    startDate.ToString("yyyy-MM-dd"), // EndDate
                    ConvertToTime(start), // StartTime
                    ConvertToTime(end), // EndTime
                    operationId // Name (operationId)
                );
            };

            // Extract and deserialize the RoomSchedule from the JSON string
            string roomScheduleRaw = parsedJson["RoomSchedule"].GetString(); // Get the RoomSchedule string
            var roomScheduleList = JsonSerializer.Deserialize<List<JsonElement>>(roomScheduleRaw); // Deserialize into a list of JsonElement

            // Convert the RoomSchedule to SlotsDto
            var roomScheduleDict = new Dictionary<string, List<SlotsDto>>
            {
                { roomId, roomScheduleList.Select(toSlotsDto).ToList() }
            };

            // Deserialize StaffSchedule
            // Get the raw JSON string for StaffSchedule
            var staffScheduleRaw = parsedJson["StaffSchedule"].GetString();

            // Deserialize the raw JSON string into a list of dictionaries (each dictionary representing a staff member)
            var staffScheduleList = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(staffScheduleRaw);

            // Convert the StaffSchedule to a dictionary of SlotsDto
            var staffScheduleDict = staffScheduleList
                .ToDictionary(
                    staff => staff["id"].GetString() ?? "unknown", // Use "unknown" if id is null
                    staff => staff["slots"].EnumerateArray().Select(toSlotsDto).ToList() // Handle empty slots gracefully
                );

            // Create SchedulesDto instance
            var schedulesDto = new SchedulesDto(date, roomId, roomScheduleDict, staffScheduleDict);

            // Return as a list
            return new List<SchedulesDto> { schedulesDto };
        }

    }
}
