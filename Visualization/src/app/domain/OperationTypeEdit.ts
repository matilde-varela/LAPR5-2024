import { Phase } from "./Phase";
import { RequiredStaff } from "./RequiredStaff";

export class OperationTypeEditEntity {
    id: string;
    name: string;
    estimatedDuration: number;
    status: boolean;
    requiredStaff: RequiredStaff[];
    phases: Phase[];

    constructor(
        id: string = '',
        name: string = '',
        estimatedDuration: number = 0,
        status: boolean = false,
        requiredStaff: RequiredStaff[] = [],
        phases: Phase[] = []
    ) {
        this.id = id;
        this.name = name;
        this.estimatedDuration = estimatedDuration;
        this.status = status;
        this.requiredStaff = requiredStaff;
        this.phases = phases;
    }

    // Method to add a staff member
    addStaff(staff: RequiredStaff): void {
        this.requiredStaff.push(staff);
    }

    // Method to remove a staff member by index
    removeStaff(index: number): void {
        if (index >= 0 && index < this.requiredStaff.length) {
            this.requiredStaff.splice(index, 1);
        }
    }

    // Method to add a phase
    addPhase(phase: Phase): void {
        this.phases.push(phase);
    }

    // Method to remove a phase by index
    removePhase(index: number): void {
        if (index >= 0 && index < this.phases.length) {
            this.phases.splice(index, 1);
        }
    }

    // Method to calculate total duration
    calculateTotalDuration(): number {
        return this.phases.reduce((total, phase) => total + (phase.duration || 0), 0);
    }
}
