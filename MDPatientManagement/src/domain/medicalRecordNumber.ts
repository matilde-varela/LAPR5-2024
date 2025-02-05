import { ValueObject } from "../core/domain/ValueObject";
import { Result } from "../core/logic/Result";

interface MedicalRecordNumberProps {
    medicalRecordNumber: string;
}

export class MedicalRecordNumber extends ValueObject<MedicalRecordNumberProps> {

    get value(): string {
        return this.props.medicalRecordNumber;
    }

    public constructor(props: MedicalRecordNumberProps) {
        super(props);
    }

    public static create(props: MedicalRecordNumberProps): Result<MedicalRecordNumber> {

        const medicalRecordNumberDto = props.medicalRecordNumber;

        const year = medicalRecordNumberDto.slice(0, 4);
        const month = medicalRecordNumberDto.slice(4, 6);
        const seqNumber = medicalRecordNumberDto.slice(6);

        if (!!medicalRecordNumberDto === false || medicalRecordNumberDto.length < 10) {
            return Result.fail<MedicalRecordNumber>('Invalid Medical Record Number format.')
        } else if (isNaN(Number(year)) || isNaN(Number(month)) || isNaN(Number(seqNumber))) {
            return Result.fail<MedicalRecordNumber>('Invalid year, month, or sequential number format.');
        } else if (seqNumber.length !== 6) {
            return Result.fail<MedicalRecordNumber>('Sequential number must be a 6-digit number.');
        } else {
            return Result.ok<MedicalRecordNumber>(new MedicalRecordNumber({
                medicalRecordNumber: props.medicalRecordNumber,
            }));
        }
    }

}