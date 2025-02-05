import { Slots } from "./slots";

export interface StaffWithFunction {
    id : string;
    name : string;
    phone : string;
    email : string;
    address : string;
    function : string;
    specializationId : string;
    slots : Slots[];
    status : string;
}
