import { expect } from 'chai';
import { it } from 'mocha';
import { AllergyCode } from '../../../src/domain/allergyCode';

describe('Allergy Code', () => {

    it('should successfully create an AllergyCode with valid ICD-11 code', () => {
        const validCode = 'BZ04.0';
        const result = AllergyCode.create(validCode);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue()).to.be.instanceOf(AllergyCode);
        expect(result.getValue().toString()).to.equal(validCode);
    });

    it('should fail to create an AllergyCode if code is invalid', () => {
        const invalidCode = 'INVALID_CODE';
        const result = AllergyCode.create(invalidCode);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('The provided code does not respect the ICD-11 format.');
    });
});