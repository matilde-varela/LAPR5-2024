import "reflect-metadata";
import { it } from 'mocha';
import sinon from "sinon";
import { Container } from "typedi";
import { IAllergyDTO } from "../../../src/dto/IAllergyDTO";
import AllergyController from "../../../src/controllers/allergyController";
import { IAllergyQueryFilterParametersDTO } from "../../../src/dto/IAllergyQueryFilterParametersDTO";
import { Response, Request, NextFunction } from 'express';
import IAllergyService from "../../../src/services/IServices/IAllergyService";
import { Result } from "../../../src/core/logic/Result";
import { IAllergyUpdateDTO } from "../../../src/dto/IAllergyUpdateDTO";

describe("AllergyController Unit Tests", function () {
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

        let allergyServiceClass = require("../../../src/services/allergyService").default;
        let allergyServiceInstance = Container.get(allergyServiceClass);
        Container.set("AllergyService", allergyServiceInstance);
    });

    afterEach(() => {
        sandbox.restore();
        sinon.restore();
    });

    it("should create an allergy successfully with allergyService stub", async () => {
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
    
        let next: Partial<NextFunction> = () => { };
    
        let allergyServiceInstance = Container.get("AllergyService") as IAllergyService;
        sinon.stub(allergyServiceInstance, "createAllergy").returns(Result.ok<IAllergyDTO>(allergyDTO));
    
        const ctrl = new AllergyController(allergyServiceInstance as IAllergyService);
    
        // Act
        await ctrl.createAllergy(<Request>req, <Response>res, <NextFunction>next);
    
        // Assert
        sinon.assert.calledOnce(res.json);
        sinon.assert.calledOnce(res.json);
        sinon.assert.calledWith(res.json, sinon.match(allergyDTO)); 
    });

    it("should return 402 status if allergy creation fails", async function () {
        const allergyDTO: IAllergyDTO = {
            code: 'BZ04',
            designation: 'Peanut Allergy',
            description: 'An allergic reaction to peanuts.',
        };
    
        let body = allergyDTO;
        let req: Partial<Request> = {};
        req.body = body;
    
        let res: Partial<Response> = {
            json: sinon.spy(),
            status: sinon.stub().returnsThis() 
        };
    
        let next: Partial<NextFunction> = () => { };

        const allergyServiceInstance = Container.get("AllergyService") as IAllergyService;
        sinon.stub(allergyServiceInstance, "createAllergy").resolves(Result.fail<IAllergyDTO>("Error creating allergy"));
    
        const ctrl = new AllergyController(allergyServiceInstance as IAllergyService);
    
        // Act
        await ctrl.createAllergy(<Request>req, <Response>res, <NextFunction>next);
    
        // Assert
        sinon.assert.calledOnce(res.status as sinon.SinonSpy);
        sinon.assert.calledWith(res.status as sinon.SinonSpy, 402);
        sinon.assert.notCalled(res.json); 
    });
    

    it("should call next with error if an exception is thrown", async function () {
        const allergyDTO: IAllergyDTO = {
            code: 'BZ04',
            designation: 'Peanut Allergy',
            description: 'An allergic reaction to peanuts.',
        };

        const req: Partial<Request> = {
            body: allergyDTO,
        };
        const res: Partial<Response> = {
            json: sinon.spy()
        };
        const next: Partial<NextFunction> = sinon.spy();


        const allergyServiceInstance = Container.get("AllergyService") as IAllergyService;
        sinon.stub(allergyServiceInstance, "createAllergy").rejects(new Error("Unexpected error"));

        const ctrl = new AllergyController(allergyServiceInstance as IAllergyService);
        // Act
        await ctrl.createAllergy(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(next);
        sinon.assert.calledWith(next, sinon.match.instanceOf(Error));
    });

    it("should return 201 status and allergy DTO when allergy is updated successfully", async function () {
        const allergyDTO: IAllergyUpdateDTO = {
            code: 'BZ04',
            designation: 'Peanut Allergy',
            description: 'An allergic reaction to peanuts.',
        };

        let body = allergyDTO;
        let req: Partial<Request> = {};
        req.body = body;

        let res: Partial<Response> = {
            json: sinon.spy(),
            status: sinon.stub().returnsThis() // Mock status method and chain it
        };

        let next: Partial<NextFunction> = () => { };

        const allergyServiceInstance = Container.get("AllergyService") as IAllergyService;
        sinon.stub(allergyServiceInstance, "updateAllergy").resolves(Result.ok<IAllergyDTO>(allergyDTO));

        const ctrl = new AllergyController(allergyServiceInstance as IAllergyService);

        // Act
        await ctrl.updateAllergy(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.status as sinon.SinonSpy);
        sinon.assert.calledWith(res.status as sinon.SinonSpy, 201);
        sinon.assert.calledOnce(res.json);
        sinon.assert.calledWith(res.json, sinon.match(allergyDTO));
    });

    it("should return 404 status if allergy update fails", async function () {
        const allergyDTO: IAllergyUpdateDTO = {
            code: 'BZ04',
            designation: 'Peanut Allergy',
            description: 'An allergic reaction to peanuts.',
        };

        let body = allergyDTO;
        let req: Partial<Request> = {};
        req.body = body;

        let res: Partial<Response> = {
            json: sinon.spy(),
            status: sinon.stub().returnsThis()
        };

        let next: Partial<NextFunction> = () => { };

        const allergyServiceInstance = Container.get("AllergyService") as IAllergyService;
        sinon.stub(allergyServiceInstance, "updateAllergy").resolves(Result.fail<IAllergyDTO>("Allergy not found"));

        const ctrl = new AllergyController(allergyServiceInstance as IAllergyService);

        // Act
        await ctrl.updateAllergy(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.status as sinon.SinonSpy);
        sinon.assert.calledWith(res.status as sinon.SinonSpy, 404);
        sinon.assert.notCalled(res.json);
    });

    it("should call next with error if an exception is thrown during allergy update", async function () {
        const allergyDTO: IAllergyUpdateDTO = {
            code: 'BZ04',
            designation: 'Peanut Allergy',
            description: 'An allergic reaction to peanuts.',
        };

        const req: Partial<Request> = {
            body: allergyDTO,
        };
        const res: Partial<Response> = {
            json: sinon.spy()
        };
        const next: Partial<NextFunction> = sinon.spy();

        const allergyServiceInstance = Container.get("AllergyService") as IAllergyService;
        sinon.stub(allergyServiceInstance, "updateAllergy").rejects(new Error("Unexpected error"));

        const ctrl = new AllergyController(allergyServiceInstance as IAllergyService);

        // Act
        await ctrl.updateAllergy(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(next);
        sinon.assert.calledWith(next, sinon.match.instanceOf(Error)); 
    });

    it("should return allergies when found", async function () {
        const filter: IAllergyQueryFilterParametersDTO = {
            queryfilters: [
                {
                    code: 'BZ02.2', // Optional: provide the allergy code if needed
                }
            ]
        };
        const req: Partial<Request> = {
            body: filter,
        };
        const res: Partial<Response> = {
            json: sinon.spy(),
            status: sinon.stub().returnsThis() // Mock `status` method and chain it
        };
        const next: Partial<NextFunction> = sinon.spy();
    
        const allergiesDto: IAllergyDTO[] = [
            {
                code: 'BZ02.2',
                designation: 'AllergyDesignation1',
                description: 'AllergyDescription1',
            }
        ];
    
        const allergyServiceInstance = Container.get("AllergyService") as IAllergyService;
        sinon.stub(allergyServiceInstance, "getAllergiesByFilters").returns(Result.ok<IAllergyDTO[]>(allergiesDto));
    
        const ctrl = new AllergyController(allergyServiceInstance as IAllergyService);
    
        await ctrl.getAllergiesByFilter(<Request>req, <Response>res, <NextFunction>next);
    
        // Assert that `res.json` is called with the correct allergies
        sinon.assert.calledOnce(res.json as sinon.SinonSpy);
        sinon.assert.calledWith(res.json as sinon.SinonSpy, allergiesDto);
    });
    
});
