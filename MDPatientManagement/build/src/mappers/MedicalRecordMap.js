"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.MedicalRecordMap = void 0;
const Mapper_1 = require("../core/infra/Mapper");
const medicalRecord_1 = require("../domain/medicalRecord");
class MedicalRecordMap extends Mapper_1.Mapper {
    static toDTO(medicalRecord) {
        // console.log(medicalRecord)
        return {
            id: medicalRecord.id.toString(),
            medicalRecordNumber: medicalRecord.props.medicalRecordNumber.value,
            medicalConditions: medicalRecord.medicalConditions.map(condition => condition.toString()),
            allergies: medicalRecord.allergies.map(allergy => allergy.toString()),
            description: medicalRecord.description
        };
    }
    static toPersistence(medicalRecord) {
        return {
            domainId: medicalRecord.id.toString(),
            id: medicalRecord.id.toString(),
            medicalRecordNumber: medicalRecord.props.medicalRecordNumber.value,
            medicalConditions: medicalRecord.medicalConditions,
            allergies: medicalRecord.allergies,
            description: medicalRecord.description
        };
    }
    static toDomain(medicalRecord) {
        const medicalRecordOrError = medicalRecord_1.MedicalRecord.create(medicalRecord);
        medicalRecordOrError.isFailure ? console.log(medicalRecordOrError.error) : '';
        return medicalRecordOrError.isSuccess ? medicalRecordOrError.getValue() : null;
    }
}
exports.MedicalRecordMap = MedicalRecordMap;
//# sourceMappingURL=MedicalRecordMap.js.map