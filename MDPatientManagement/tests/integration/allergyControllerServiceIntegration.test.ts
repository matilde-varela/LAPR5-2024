import "reflect-metadata";
import { it } from 'mocha';
import sinon from "sinon";
import { Container } from "typedi";
import { IAllergyDTO } from "../../src/dto/IAllergyDTO";
import AllergyController from "../../src/controllers/allergyController";
import { IAllergyQueryFilterParametersDTO } from "../../src/dto/IAllergyQueryFilterParametersDTO";
import { Response, Request, NextFunction } from 'express';
import IAllergyService from "../../src/services/IServices/IAllergyService";
import { Result } from "../../src/core/logic/Result";
import IAllergyRepo from "../../src/services/IRepos/IAllergyRepo";
import { AllergyMap } from "../../src/mappers/AllergyMap";
import { Allergy } from "../../src/domain/allergy";
import { IAllergyUpdateDTO } from "../../src/dto/IAllergyUpdateDTO";

describe("Allergy Controller+Service Integration Tests", function () {
    const sandbox = sinon.createSandbox();
    this.timeout(10000);

    beforeEach(() => {
        Container.reset();

        const mockLogger = {
            info: sandbox.stub(),
            error: sandbox.stub(),
            warn: sandbox.stub(),
            debug: sandbox.stub(),
        };
        Container.set("logger", mockLogger);

        let allergySchemaInstance = require("../../src/persistence/schemas/allergySchema").default;
        Container.set("allergySchema", allergySchemaInstance);

        let allergyRepoClass = require("../../src/repos/allergyRepo").default;
        let allergyRepoInstance = Container.get(allergyRepoClass);
        Container.set("AllergyRepo", allergyRepoInstance);

        let allergyServiceClass = require("../../src/services/allergyService").default;
        let allergyServiceInstance = Container.get(allergyServiceClass);
        Container.set("AllergyService", allergyServiceInstance);
    });

    afterEach(() => {
        sandbox.restore();
        sinon.restore();
    });

    it("should create an allergy successfully with allergyRepo and allergy stub", async () => {
        // Arrange
        const allergyDTO: IAllergyDTO = {
            code: 'BZ04',
            designation: 'Peanut Allergy',
            description: 'An allergic reaction to peanuts.',
        };
    
        let body = allergyDTO;
        let req: Partial<Request> = {};
        req.body = body;
        let res: Partial<Response> = {
            json: sinon.spy()
        };
        const next: Partial<NextFunction> = sinon.spy();
    
        const createStub = sinon.stub(Allergy, "create").resolves({
            isFailure: false,
            getValue: () => allergyDTO,
        });
    
        const mapStub = sinon.stub(AllergyMap, "toDTO").returns(allergyDTO);
    
        const allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        const saveStub = sinon.stub(allergyRepoInstance, "save").resolves(allergyDTO);
    
        const allergyServiceInstance = Container.get("AllergyService") as IAllergyService;
        const ctrl = new AllergyController(allergyServiceInstance);
    
        // Act
        await ctrl.createAllergy(<Request>req, <Response>res, <NextFunction>next);
    
        // Assert
        sinon.assert.calledOnce(createStub);
        sinon.assert.calledOnce(saveStub);
        sinon.assert.calledOnce(mapStub);
        sinon.assert.calledOnce(res.json);
        sinon.assert.calledWith(res.json, sinon.match(allergyDTO));
    });

    it("should update an allergy successfully with allergyRepo and allergy stub", async () => {
        // Arrange
        const allergyDTO: IAllergyUpdateDTO = {
            code: 'BZ04',
            designation: 'Peanut Allergy Updated',
            description: 'An allergic reaction to peanuts, now updated.',
        };

        const updatedAllergyDTO: IAllergyUpdateDTO = {
            code: 'BZ04',
            designation: 'Seafood Allergy',
            description: 'Green skin and mucus.',
        };
    
        const req: Partial<Request> = { body: updatedAllergyDTO};
        const res: Partial<Response> = {
            json: sinon.spy(),
            status: sinon.stub().returnsThis()
        };
        const next: Partial<NextFunction> = sinon.spy();
    
        const mockAllergy = {
            code: allergyDTO.code,
            designation: allergyDTO.designation,
            description: allergyDTO.description,
            changeDesignation: sinon.spy(),
            changeDescription: sinon.spy()
        };
    
        const allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        const findByCodeStub = sinon.stub(allergyRepoInstance, "findByCode").resolves(mockAllergy);
        const saveStub = sinon.stub(allergyRepoInstance, "save").resolves(mockAllergy);
        const mapStub = sinon.stub(AllergyMap, "toDTO").returns(updatedAllergyDTO);

        const allergyServiceInstance = Container.get("AllergyService") as IAllergyService;
        const ctrl = new AllergyController(allergyServiceInstance);
    
        // Act
        await ctrl.updateAllergy(<Request>req, <Response>res, <NextFunction>next);
    
        // Assert
        sinon.assert.calledOnce(findByCodeStub);
        sinon.assert.calledOnce(saveStub);
        sinon.assert.calledOnce(mapStub);
        sinon.assert.calledOnce(res.json); 
        sinon.assert.calledWith(res.json, updatedAllergyDTO); 
    });
    
      it("should return error 404 if allergy not found in updateAllergy", async function () {
        // Arrange
        const allergyDTO: IAllergyUpdateDTO = {
          code: 'BZ04',
          designation: 'Peanut Allergy Updated',
          description: 'An allergic reaction to peanuts, now updated.',
        };
    
        let body = allergyDTO;
        let req: Partial<Request> = {};
        req.body = body;
        let res: Partial<Response> = {
          json: sinon.spy(),
          status: sinon.stub().returnsThis()
        };
        const next: Partial<NextFunction> = sinon.spy();
    
        const allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        const findByCodeStub = sinon.stub(allergyRepoInstance, "findByCode").resolves(null);
        const allergyServiceInstance = Container.get("AllergyService") as IAllergyService;
        const ctrl = new AllergyController(allergyServiceInstance);
    
        // Act
        await ctrl.updateAllergy(<Request>req, <Response>res, <NextFunction>next);
    
        // Assert
        sinon.assert.calledOnce(findByCodeStub);
        sinon.assert.calledOnce(res.status);
        sinon.assert.calledWith(res.status, 404);
        sinon.assert.notCalled(res.json);
      });
    
      it("should call next with error if an exception occurs during updateAllergy", async function () {
        // Arrange
        const allergyDTO: IAllergyUpdateDTO = {
          code: 'BZ04',
          designation: 'Peanut Allergy Updated',
          description: 'An allergic reaction to peanuts, now updated.',
        };
    
        let body = allergyDTO;
        let req: Partial<Request> = {};
        req.body = body;
        let res: Partial<Response> = {
          json: sinon.spy()
        };
        const next: Partial<NextFunction> = sinon.spy();
    
        const allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        const findByCodeStub = sinon.stub(allergyRepoInstance, "findByCode").rejects(new Error("Unexpected error"));
    
        const allergyServiceInstance = Container.get("AllergyService") as IAllergyService;
        const ctrl = new AllergyController(allergyServiceInstance);
    
        // Act
        await ctrl.updateAllergy(<Request>req, <Response>res, <NextFunction>next);
    
        // Assert
        sinon.assert.calledOnce(next);
        sinon.assert.calledOnce(findByCodeStub);
        sinon.assert.calledWith(next, sinon.match.instanceOf(Error));
      });


    it("should return allergies when found with allergyRepo and allergy stub", async function () {
        const filter: IAllergyQueryFilterParametersDTO = {
            queryfilters: [{ code: 'BZ02.2' }],
        };
        const req: Partial<Request> = { body: filter };
        const res: Partial<Response> = { json: sinon.spy(), status: sinon.stub().returnsThis()};
        const next: Partial<NextFunction> = sinon.spy();
        const allergies = [{ code: 'BZ02.2', designation: 'AllergyDesignation1', description: 'AllergyDescription1' }];
        const allergyDto: IAllergyDTO = { code: 'BZ02.2', designation: 'AllergyDesignation1', description: 'AllergyDescription1' };
        const allergiesDto = [allergyDto];
        const allergyRepoInstance = Container.get("AllergyRepo") as IAllergyRepo;
        const repoStub = sinon.stub(allergyRepoInstance, "findAllByParameters").returns(allergies);
        const mapStub = sinon.stub(AllergyMap, "toDTO").returns(allergyDto);
    
        const allergyServiceInstance = Container.get("AllergyService") as IAllergyService;
        const ctrl = new AllergyController(allergyServiceInstance);
        await ctrl.getAllergiesByFilter(<Request>req, <Response>res, <NextFunction>next);
    
        // Assert
        sinon.assert.calledOnce(repoStub);
        sinon.assert.calledOnce(mapStub);
        sinon.assert.calledOnce(res.json);
        sinon.assert.calledWith(res.json, allergiesDto);
    });

});
