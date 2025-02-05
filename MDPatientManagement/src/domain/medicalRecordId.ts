import { UniqueEntityID } from "../core/domain/UniqueEntityID";

export class MedicalRecordId extends UniqueEntityID {
  public constructor(id: string) {
    super(id);
  }
}