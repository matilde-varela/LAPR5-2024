import { expect } from 'chai';
import { it } from 'mocha';
import { AllergyDesignation } from '../../../src/domain/allergyDesignation';


describe('Allergy Designation', () => {
    it('should successfully create an AllergyDesignation with valid data', () => {
        const validDesignation = 'Peanut Allergy';

        const result = AllergyDesignation.create(validDesignation);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue()).to.be.instanceOf(AllergyDesignation);

        const designation = result.getValue() as AllergyDesignation;
        expect(designation.value).to.equal(validDesignation);
    });

    it('should fail if designation is null', () => {
        const invalidDesignation = null;

        const result = AllergyDesignation.create(invalidDesignation as unknown as string);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('designation is null or undefined');
    });

    it('should fail if designation is undefined', () => {
        const invalidDesignation = undefined;

        const result = AllergyDesignation.create(invalidDesignation as unknown as string);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('designation is null or undefined');
    });

});