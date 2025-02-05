import { expect } from 'chai';
import { AllergyDescription } from '../../../src/domain/allergyDescription';
import { it } from 'mocha';

describe('AllergyDescription Value Object', () => {

    it('should successfully create an AllergyDescription with valid data', () => {
        const validDescription = 'A severe reaction to peanuts causing anaphylaxis.';

        const result = AllergyDescription.create(validDescription);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue()).to.be.instanceOf(AllergyDescription);

        const description = result.getValue() as AllergyDescription;
        expect(description.value).to.equal(validDescription);
    });

    it('should fail if description is null', () => {
        const invalidDescription = null;

        const result = AllergyDescription.create(invalidDescription as unknown as string);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('description is null or undefined');
    });

    it('should fail if description is undefined', () => {
        const invalidDescription = undefined;

        const result = AllergyDescription.create(invalidDescription as unknown as string);

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('description is null or undefined');
    });

});