import { Slots } from "./slots"

export interface CreateRoom {
    roomNumber : string
    typeDesignation : string
    capacity : string
    availableEquipment : string[]
    maintenanceSlots : string[]
}