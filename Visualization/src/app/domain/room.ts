import { Slots } from "./slots"

export interface Room {
    roomNumber : string
    type : string
    capacity : number
    availableEquipment : string[]
    currentStatus : string
    maintenanceSlots : Slots[]
}