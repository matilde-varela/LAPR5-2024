import { expect } from 'chai';
import { MedicalCondition }  from '../../../src/domain/medicalCondition';
import IMedicalConditionDTO from '../../../src/dto/IMedicalConditionDTO';
import {it} from 'mocha';

describe('MedicalCondition Aggregate', () => {

    it('should successfunpmlly create a MedicalCondition with valid data', () => {
        const dto: IMedicalConditionDTO = {
            id: 'FB70.0',
            designation: 'Diabetes',
            description: 'A chronic condition that affects the way the body processes blood sugar.',
            symptoms: ['Frequent urination', 'Increased thirst']
        };

        const result = MedicalCondition.create(dto);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue()).to.be.instanceOf(MedicalCondition);

        const medicalCondition = result.getValue() as MedicalCondition;
        expect(medicalCondition.id.toString()).to.equal('FB70.0');
        expect(medicalCondition.designation).to.equal('Diabetes');
        expect(medicalCondition.description).to.equal('A chronic condition that affects the way the body processes blood sugar.');
        expect(medicalCondition.symptoms).to.deep.equal(['Frequent urination', 'Increased thirst']);
    });

    it('should fail if designation is empty', () => {
        const dto: IMedicalConditionDTO = {
            id: 'FB70.0',
            designation: '',
            description: 'Valid description',
            symptoms: ['Symptom 1']
        };

        const result = MedicalCondition.create(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('Must provide a designation.');
    });

    it('should fail if designation is too long', () => {
        const dto: IMedicalConditionDTO = {
            id: 'FB70.0',
            designation: 'A'.repeat(101), // 101 characters
            description: 'Valid description',
            symptoms: ['Symptom 1']
        };

        const result = MedicalCondition.create(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('Must provide a smaller designation.');
    });

    it('should fail if id is empty', () => {
        const dto: IMedicalConditionDTO = {
            id: '',
            designation: 'Valid designation',
            description: 'Valid description',
            symptoms: ['Symptom 1']
        };

        const result = MedicalCondition.create(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('Must provide an id.');
    });
    
    it('should fail if description is empty', () => {
        const dto: IMedicalConditionDTO = {
            id: 'FB70.0',
            designation: 'Valid designation',
            description: '',
            symptoms: ['Symptom 1']
        };

        const result = MedicalCondition.create(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('Must provide a description.');
    });

    it('should fail if description is too long', () => {
        const dto: IMedicalConditionDTO = {
            id: 'FB70.0',
            designation: 'Valid designation',
            description: 'A'.repeat(2049), // 2049 characters
            symptoms: ['Symptom 1']
        };

        const result = MedicalCondition.create(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('Must provide a smaller description.');
    });

    it('should fail if symptoms are empty', () => {
        const dto: IMedicalConditionDTO = {
            id: 'FB70.0',
            designation: 'Valid designation',
            description: 'Valid description',
            symptoms: []
        };

        const result = MedicalCondition.create(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('Must provide at least one symptom.');
    });
});
