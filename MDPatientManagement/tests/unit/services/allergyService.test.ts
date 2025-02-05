import "reflect-metadata";
import { expect } from "chai";
import { it } from 'mocha';
import sinon from "sinon";
import { Container } from "typedi";
import { IAllergyDTO } from "../../../src/dto/IAllergyDTO";
import IAllergyRepo from "../../../src/services/IRepos/IAllergyRepo";
import AllergyService from "../../../src/services/allergyService";
import { AllergyMap } from "../../../src/mappers/AllergyMap";
import { IAllergyQueryFilterParametersDTO } from "../../../src/dto/IAllergyQueryFilterParametersDTO";
import { Result } from "../../../src/core/logic/Result";
import { Allergy } from "../../../src/domain/allergy";
import { IAllergyUpdateDTO } from "../../../src/dto/IAllergyUpdateDTO";

describe("AllergyService Unit Tests", function () {
    const sandbox = sinon.createSandbox();
    this.timeout(5000);

    beforeEach(() => {
        Container.reset();

        const mockLogger = {
            info: sandbox.stub(),
            error: sandbox.stub(),
            warn: sandbox.stub(),
            debug: sandbox.stub(),
        };
        Container.set("logger", mockLogger);

        let allergySchemaInstance = require("../../../src/persistence/schemas/allergySchema").default;
        Container.set("allergySchema", allergySchemaInstance);

        let allergyRepoClass = require("../../../src/repos/allergyRepo").default;
        let allergyRepoInstance = Container.get(allergyRepoClass);
        Container.set("AllergyRepo", allergyRepoInstance);
    });

    afterEach(() => {
        sandbox.restore();
        sinon.restore();
    });

    it("should return a success when allergy is created and saved", async function () {
        const allergyDTO: IAllergyDTO = {
            code: 'BZ04',
            designation: 'Peanut Allergy',
            description: 'An allergic reaction to peanuts.',
        };

        const allergyInstance = { code: 'BZ04', designation: 'Peanut Allergy', description: 'An allergic reaction to peanuts.' };
        sinon.stub(Allergy, "create").resolves(Result.ok(allergyInstance));

        let allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        sinon.stub(allergyRepoInstance, "save").resolves();


        sinon.stub(AllergyMap, "toDTO").returns(allergyDTO);

        const service = new AllergyService(allergyRepoInstance as IAllergyRepo);
        const result = await service.createAllergy(allergyDTO);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue()).to.deep.equal(allergyDTO);
    });

    it("should return a failure if allergy creation fails", async function () {
        const allergyDTO: IAllergyDTO = {
            code: 'BZ04',
            designation: 'Peanut Allergy',
            description: 'An allergic reaction to peanuts.',
        };

        const errorMessage = 'Invalid allergy designation';
        sinon.stub(Allergy, "create").resolves(Result.fail(errorMessage));

        let allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        sinon.stub(allergyRepoInstance, "save").resolves();

        const service = new AllergyService(allergyRepoInstance as IAllergyRepo);
        const result = await service.createAllergy(allergyDTO);

        expect(result.isFailure).to.be.true;
        expect(result.errorValue()).to.equal(errorMessage);
    });

    it("should throw an error if there is an exception in the repository during save", async function () {
        const allergyDTO: IAllergyDTO = {
            code: 'BZ04',
            designation: 'Peanut Allergy',
            description: 'An allergic reaction to peanuts.',
        };

        const allergyInstance = { code: 'BZ04', designation: 'Peanut Allergy', description: 'An allergic reaction to peanuts.' };
        sinon.stub(Allergy, "create").resolves(Result.ok(allergyInstance));

        let allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        sinon.stub(allergyRepoInstance, "save").rejects(new Error("Database error"));

        const service = new AllergyService(allergyRepoInstance as IAllergyRepo);

        try {
            await service.createAllergy(allergyDTO);
            expect.fail("Expected error to be thrown");
        } catch (error) {
            expect(error.message).to.equal("Database error");
        }
    });

    it("should update an allergy successfully", async function () {
        const allergyDTO: IAllergyUpdateDTO = {
            code: 'BZ04',
            designation: 'Seafood Allergy',
            description: 'Green skin and mucus.',
        };

        const existingAllergy = {
            code: 'BZ04',
            designation: 'Peanut Allergy',
            description: 'An allergic reaction to peanuts.',
            changeDesignation: sinon.stub(),
            changeDescription: sinon.stub(),
        };

        let allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        sinon.stub(allergyRepoInstance, "findByCode").resolves(existingAllergy);
        sinon.stub(allergyRepoInstance, "save").resolves();
        sinon.stub(AllergyMap, "toDTO").returns({
            code: allergyDTO.code,
            designation: allergyDTO.designation,
            description: allergyDTO.description,
        });

        const service = new AllergyService(allergyRepoInstance as IAllergyRepo);
        const result = await service.updateAllergy(allergyDTO);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue()).to.deep.equal({
            code: allergyDTO.code,
            designation: allergyDTO.designation,
            description: allergyDTO.description,
        });
        expect(existingAllergy.changeDesignation.calledWith(allergyDTO.designation)).to.be.true;
        expect(existingAllergy.changeDescription.calledWith(allergyDTO.description)).to.be.true;
    });

    it("should return a failure if allergy is not found", async function () {
        const allergyDTO: IAllergyUpdateDTO = {
            code: 'BZ04',
            designation: 'Seafood Allergy',
            description: 'Green skin and mucus.',
        };

        let allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        sinon.stub(allergyRepoInstance, "findByCode").resolves(null);

        const service = new AllergyService(allergyRepoInstance as IAllergyRepo);
        const result = await service.updateAllergy(allergyDTO);

        expect(result.isFailure).to.be.true;
        expect(result.errorValue()).to.equal("Allergy not found");
    });

    it("should throw an error if there is an exception in the repository during save", async function () {
        const allergyDTO: IAllergyUpdateDTO = {
            code: 'BZ04',
            designation: 'Seafood Allergy',
            description: 'Green skin and mucus.',
        };

        const existingAllergy = {
            code: 'BZ04',
            designation: 'Peanut Allergy',
            description: 'An allergic reaction to peanuts.',
            changeDesignation: sinon.stub(),
            changeDescription: sinon.stub(),
        };

        let allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        sinon.stub(allergyRepoInstance, "findByCode").resolves(existingAllergy);
        sinon.stub(allergyRepoInstance, "save").rejects(new Error("Database error"));

        const service = new AllergyService(allergyRepoInstance as IAllergyRepo);

        try {
            await service.updateAllergy(allergyDTO);
            expect.fail("Expected error to be thrown");
        } catch (error) {
            expect(error.message).to.equal("Database error");
        }
    });

    

    it("should return allergies when found", async function () {
        const filter: IAllergyQueryFilterParametersDTO = {
            queryfilters: [
                {
                    code: 'BZ02.2',       // Optional: provide the allergy code if needed
                }
            ]
        };
        const allergies = [
            {
                code: 'BZ02.2',
                designation: 'AllergyDesignation1',
                description: 'AllergyDescription1'
            }
        ];

        const allergiesDto: IAllergyDTO[] = [
            {
                code: 'BZ02.2',
                designation: 'AllergyDesignation1',
                description: 'AllergyDescription1'
            }
        ];

        let allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        sinon.stub(allergyRepoInstance, "findAllByParameters").resolves(allergies);

        sinon.stub(AllergyMap, "toDTO").returns(allergiesDto);

        const service = new AllergyService(allergyRepoInstance as IAllergyRepo);
        const result = await service.getAllergiesByFilters(filter);

        expect(result.isSuccess).to.be.true;
        expect(result.getValue()).to.have.lengthOf(1);
    });

    it("should return a failure if no allergies are found", async function () {
        const filter: IAllergyQueryFilterParametersDTO = {
            queryfilters: [
                {
                    code: 'BZ02.2',
                }
            ]
        };
        let allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        sinon.stub(allergyRepoInstance, "findAllByParameters").resolves([]);

        const service = new AllergyService(allergyRepoInstance as IAllergyRepo);
        const result = await service.getAllergiesByFilters(filter);

        expect(result.isSuccess).to.be.false;
        expect(result.errorValue()).to.equal("Allergy not found");
    });

    it("should throw an error if there is an exception in the repository", async function () {
        const filter: IAllergyQueryFilterParametersDTO = { queryfilters: [] };
        let allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        sinon.stub(allergyRepoInstance, "findAllByParameters").rejects(new Error("Database error"));

        try {
            const service = new AllergyService(allergyRepoInstance as IAllergyRepo);
            await service.getAllergiesByFilters(filter);
            expect.fail("Expected error to be thrown");
        } catch (error) {
            expect(error.message).to.equal("Failed to fetch allergy: Database error");
        }
    });

});
