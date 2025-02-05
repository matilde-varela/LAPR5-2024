"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.MedicalRecordNumber = void 0;
const ValueObject_1 = require("../core/domain/ValueObject");
const Result_1 = require("../core/logic/Result");
class MedicalRecordNumber extends ValueObject_1.ValueObject {
    get value() {
        return this.props.medicalRecordNumber;
    }
    constructor(props) {
        super(props);
    }
    static create(props) {
        const medicalRecordNumberDto = props.medicalRecordNumber;
        const year = medicalRecordNumberDto.slice(0, 4);
        const month = medicalRecordNumberDto.slice(4, 6);
        const seqNumber = medicalRecordNumberDto.slice(6);
        if (!!medicalRecordNumberDto === false || medicalRecordNumberDto.length < 10) {
            return Result_1.Result.fail('Invalid Medical Record Number format.');
        }
        else if (isNaN(Number(year)) || isNaN(Number(month)) || isNaN(Number(seqNumber))) {
            return Result_1.Result.fail('Invalid year, month, or sequential number format.');
        }
        else if (seqNumber.length !== 6) {
            return Result_1.Result.fail('Sequential number must be a 6-digit number.');
        }
        else {
            return Result_1.Result.ok(new MedicalRecordNumber({
                medicalRecordNumber: props.medicalRecordNumber,
            }));
        }
    }
}
exports.MedicalRecordNumber = MedicalRecordNumber;
//# sourceMappingURL=medicalRecordNumber.js.map