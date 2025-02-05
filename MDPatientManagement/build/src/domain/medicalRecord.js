"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.MedicalRecord = void 0;
const AggregateRoot_1 = require("../core/domain/AggregateRoot");
const Result_1 = require("../core/logic/Result");
const allergyCode_1 = require("./allergyCode");
const medicalConditionId_1 = require("./medicalConditionId");
const medicalRecordId_1 = require("./medicalRecordId");
const medicalRecordNumber_1 = require("./medicalRecordNumber");
//adicionar medical record ao patient no backoffice
//mandar um token quando passo deste para o backoffice, mandar o mesmo que se recebe inicialmente no pedido
//colocar cena do auth em todos os requests do novo modulo
class MedicalRecord extends AggregateRoot_1.AggregateRoot {
    get medicalRecordId() {
        return this.id;
    }
    get patientMedicalRecordNumber() {
        return this.props.medicalRecordNumber;
    }
    set medicalRecordNumber(value) {
        this.props.medicalRecordNumber = value;
    }
    get description() {
        return this.props.description;
    }
    set description(value) {
        this.props.description = value;
    }
    get medicalConditions() {
        return this.props.medicalConditions;
    }
    set medicalConditions(values) {
        this.props.medicalConditions = values;
    }
    get allergies() {
        return this.props.allergies;
    }
    set allergies(values) {
        this.props.allergies = values;
    }
    static create(medicalRecordDTO) {
        const idDto = medicalRecordDTO.id;
        const medicalRecordNumberDto = medicalRecordDTO.medicalRecordNumber;
        const medicalConditionsDto = medicalRecordDTO.medicalConditions || [];
        const allergiesDto = medicalRecordDTO.allergies || [];
        const descriptionDto = medicalRecordDTO.description || "";
        if (!!idDto === false || idDto.length === 0) {
            return Result_1.Result.fail('Must provide an id.');
        }
        else if (!!medicalRecordNumberDto === false || medicalRecordNumberDto.length === 0) {
            return Result_1.Result.fail('Must provide an medical record number.');
        }
        else {
            const medicalRecordNumberResult = medicalRecordNumber_1.MedicalRecordNumber.create({
                medicalRecordNumber: medicalRecordNumberDto,
            });
            if (medicalRecordNumberResult.isFailure) {
                return Result_1.Result.fail(medicalRecordNumberResult.error);
            }
            const medicalRecordNumberObject = medicalRecordNumberResult.getValue();
            const medicalConditionObjects = medicalConditionsDto.map(cond => new medicalConditionId_1.MedicalConditionId(cond.toString()));
            const allergyObjects = allergiesDto.map(allergy => new allergyCode_1.AllergyCode(allergy.toString()));
            const medicalRecord = new MedicalRecord({ medicalRecordNumber: medicalRecordNumberObject, medicalConditions: medicalConditionObjects, allergies: allergyObjects, description: descriptionDto }, new medicalRecordId_1.MedicalRecordId(idDto));
            return Result_1.Result.ok(medicalRecord);
        }
    }
    changeMedicalConditions(medicalConditions) {
        if (!Array.isArray(medicalConditions)) {
            throw new Error("Invalid medical conditions array.");
        }
        this.props.medicalConditions = medicalConditions;
    }
    changeAllergies(allergies) {
        if (!Array.isArray(allergies)) {
            throw new Error("Invalid allergies array.");
        }
        this.props.allergies = allergies;
    }
    changeDescription(description) {
        if (description === null || description === undefined) {
            throw new Error("Description cannot be null or undefined.");
        }
        this.props.description = description;
    }
}
exports.MedicalRecord = MedicalRecord;
//# sourceMappingURL=medicalRecord.js.map