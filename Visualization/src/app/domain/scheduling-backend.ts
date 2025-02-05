import { SchedulingData } from "./scheduling-data";

export interface SchedulingBackend {
    roomID : string[];
    schedulingData : SchedulingData[];
    algorithm : string;
    date : string;
}
