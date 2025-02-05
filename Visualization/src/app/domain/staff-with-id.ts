import { Slots } from "./slots";

export interface StaffWithId {
    id : string;
    name : string;
    phone : string;
    email : string;
    address : string;
    specializationId : string;
    slots : Slots[];
    status : string;
}
