import { expect } from 'chai';
import { Allergy } from '../../../src/domain/allergy';
import { IAllergyDTO } from '../../../src/dto/IAllergyDTO';
import { it } from 'mocha';
import { AllergyCode } from '../../../src/domain/allergyCode';

describe('Allergy Aggregate', () => {

    it('should successfully create an Allergy with valid data', () => {
        const dto: IAllergyDTO = {
            code: 'BZ04',
            designation: 'Peanut Allergy',
            description: 'An allergic reaction to peanuts, which can cause severe symptoms.',
        };

        const result = Allergy.create(dto);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue()).to.be.instanceOf(Allergy);

        const allergy = result.getValue() as Allergy;
        expect(allergy.code.toString()).to.equal('BZ04');
        expect(allergy.designation.value).to.equal('Peanut Allergy');
        expect(allergy.description.value).to.equal('An allergic reaction to peanuts, which can cause severe symptoms.');
    });

    it('should fail if code is empty', () => {
        const dto: IAllergyDTO = {
            code: '',
            designation: 'Valid designation',
            description: 'Valid description',
        };

        const result = Allergy.create(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('Invalid allergy code');
    });

       it('should successfully create an if description is empty', () => {
        const dto: IAllergyDTO = {
            code: 'BZ04',
            designation: 'Valid designation',
            description: '',
        };

        const result = Allergy.create(dto);

        expect(result.isSuccess).to.be.true;
    });

    it('should fail if code does not respect ICD-11 format', () => {
        const dto: IAllergyDTO = {
            code: 'INVALID_CODE',
            designation: 'Valid designation',
            description: 'Valid description',
        };

        const result = Allergy.create(dto);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('Invalid allergy code');
    });

});
