import { expect } from 'chai';
import { MedicalRecord } from '../../../src/domain/medicalRecord';
import { IMedicalRecordDTO } from '../../../src/dto/IMedicalRecordDTO';
import { it } from 'mocha';
import { AllergyCode } from '../../../src/domain/allergyCode';
import { MedicalConditionId } from '../../../src/domain/medicalConditionId';

describe('MedicalRecord Aggregate', () => {
  const validDTO: IMedicalRecordDTO = {
    id: '202412000001',
    medicalRecordNumber: '202412000001',
    medicalConditions: ['FA22.0', 'FA21.0'],
    allergies: ['BZ05.3', 'BZ02.2' ],
    description: 'Patient with chronic conditions and severe allergies.'
  };

  it('should successfully create a MedicalRecord with valid data', () => {
    const result = MedicalRecord.create(validDTO);

    expect(result.isSuccess).to.be.true;
    expect(result.getValue()).to.be.instanceOf(MedicalRecord);

    const medicalRecord = result.getValue() as MedicalRecord;
    expect(medicalRecord.medicalRecordId.toString()).to.equal('202412000001');
    expect(medicalRecord.patientMedicalRecordNumber.value).to.equal('202412000001');
    expect(medicalRecord.description).to.equal('Patient with chronic conditions and severe allergies.');
    expect(medicalRecord.medicalConditions.map(c => c.toString())).to.deep.equal(['FA22.0', 'FA21.0']);
    expect(medicalRecord.allergies.map(a => a.toString())).to.deep.equal(['BZ05.3', 'BZ02.2' ]);
  });

  it('should fail if id is empty', () => {
    const dto = { ...validDTO, id: '' };

    const result = MedicalRecord.create(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal('Must provide an id.');
  });

  it('should fail if medicalRecordNumber is empty', () => {
    const dto = { ...validDTO, medicalRecordNumber: '' };

    const result = MedicalRecord.create(dto);

    expect(result.isFailure).to.be.true;
    expect(result.error).to.equal('Must provide an medical record number.');
  });

  it('should allow updating medical conditions', () => {
    const result = MedicalRecord.create(validDTO);

    expect(result.isSuccess).to.be.true;

    const medicalRecord = result.getValue() as MedicalRecord;
    const newConditions = ['FA33.0', 'FA14.0'].map(cond => new MedicalConditionId(cond));
    medicalRecord.changeMedicalConditions(newConditions);

    expect(medicalRecord.medicalConditions.map(c => c.toString())).to.deep.equal(['FA33.0', 'FA14.0']);
  });

  it('should throw an error if updating medical conditions with invalid data', () => {
    const result = MedicalRecord.create(validDTO);

    expect(result.isSuccess).to.be.true;

    const medicalRecord = result.getValue() as MedicalRecord;

    expect(() => medicalRecord.changeMedicalConditions(null as any)).to.throw('Invalid medical conditions array.');
  });

  it('should allow updating allergies', () => {
    const result = MedicalRecord.create(validDTO);

    expect(result.isSuccess).to.be.true;

    const medicalRecord = result.getValue() as MedicalRecord;
    const newAllergies = ['FA33.0', 'FA14.0'].map(allergy => new AllergyCode(allergy));
    medicalRecord.changeAllergies(newAllergies);

    expect(medicalRecord.allergies.map(a => a.toString())).to.deep.equal(['FA33.0', 'FA14.0']);
  });

  it('should throw an error if updating allergies with invalid data', () => {
    const result = MedicalRecord.create(validDTO);

    expect(result.isSuccess).to.be.true;

    const medicalRecord = result.getValue() as MedicalRecord;

    expect(() => medicalRecord.changeAllergies(null as any)).to.throw('Invalid allergies array.');
  });

  it('should allow updating description', () => {
    const result = MedicalRecord.create(validDTO);

    expect(result.isSuccess).to.be.true;

    const medicalRecord = result.getValue() as MedicalRecord;
    medicalRecord.changeDescription('Updated description for the patient.');

    expect(medicalRecord.description).to.equal('Updated description for the patient.');
  });

  it('should throw an error if updating description with null or undefined', () => {
    const result = MedicalRecord.create(validDTO);

    expect(result.isSuccess).to.be.true;

    const medicalRecord = result.getValue() as MedicalRecord;

    expect(() => medicalRecord.changeDescription(null as any)).to.throw('Description cannot be null or undefined.');
  });
});
