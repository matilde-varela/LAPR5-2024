import "reflect-metadata";
import { it } from 'mocha';
import sinon from "sinon";
import { Container } from "typedi";
import IMedicalConditionDTO from "../../src/dto/IMedicalConditionDTO";
import MedicalConditionController from "../../src/controllers/medicalConditionController";
import IMedicalConditionService from "../../src/services/IServices/IMedicalConditionService";
import { Response, Request, NextFunction } from 'express';
import { MedicalCondition } from "../../src/domain/medicalCondition";
import { MedicalConditionMap } from "../../src/mappers/MedicalConditionMap";
import IMedicalConditionRepo from "../../src/services/IRepos/IMedicalConditionRepo";

describe("MedicalCondition Controller+Service Integration Tests", function () {
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

        let medicalConditionSchemaInstance = require("../../src/persistence/schemas/medicalConditionSchema").default;
        Container.set("medicalConditionSchema", medicalConditionSchemaInstance);

        let medicalConditionRepoClass = require("../../src/repos/medicalConditionRepo").default;
        let medicalConditionRepoInstance = Container.get(medicalConditionRepoClass);
        Container.set("MedicalConditionRepo", medicalConditionRepoInstance);

        let medicalConditionServiceClass = require("../../src/services/medicalConditionService").default;
        let medicalConditionServiceInstance = Container.get(medicalConditionServiceClass);
        Container.set("MedicalConditionService", medicalConditionServiceInstance);
    });

    afterEach(() => {
        sandbox.restore();
        sinon.restore();
    });

    it("should create a medical condition successfully with medicalConditionRepo and medicalCondition stub", async () => {
        const medicalConditionDTO: IMedicalConditionDTO = {
            id: 'FB70.0',
            designation: 'valid designation',
            description: 'valid description',
            symptoms: ['Symptom 1', 'Symptom 2']
        };

        let body = medicalConditionDTO;
        let req: Partial<Request> = {};
        req.body = body;
        let res: Partial<Response> = {
            json: sinon.spy()
        };
        let next: Partial<NextFunction> = () => { };

        sinon.stub(MedicalCondition, "create").resolves({
            isFailure: false,
            getValue: () => (medicalConditionDTO),
        });

        sinon.stub(MedicalConditionMap, "toDTO").returns(medicalConditionDTO);

        let medicalConditionRepoInstance = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
        sinon.stub(medicalConditionRepoInstance, "save").resolves(medicalConditionDTO);

        let medicalConditionServiceInstance = Container.get("MedicalConditionService") as IMedicalConditionService;
        const ctrl = new MedicalConditionController(medicalConditionServiceInstance as IMedicalConditionService);
        // Act
        await ctrl.createMedicalCondition(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.json);
        sinon.assert.calledWith(res.json, sinon.match(medicalConditionDTO));
    });

    it("should return all medical conditions successfully", async () => {
        const medicalConditionDTOList: IMedicalConditionDTO[] = [
          {
            id: "FB70.0",
            designation: "Condition 1",
            description: "Description 1",
            symptoms: ["Symptom A", "Symptom B"],
          },
          {
            id: "FB71.0",
            designation: "Condition 2",
            description: "Description 2",
            symptoms: ["Symptom C", "Symptom D"],
          },
        ];
    
        const req: Partial<Request> = {};
        const res: Partial<Response> = {
          status: sinon.stub().returnsThis(),
          json: sinon.spy(),
        };
        const next: Partial<NextFunction> = sinon.spy();
    
        const medicalConditionRepoInstance = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
        sinon.stub(medicalConditionRepoInstance, "findAll").resolves([
          {
            id: "FB70.0",
            designation: "Condition 1",
            description: "Description 1",
            symptoms: ["Symptom A", "Symptom B"],
          },
          {
            id: "FB71.0",
            designation: "Condition 2",
            description: "Description 2",
            symptoms: ["Symptom C", "Symptom D"],
          },
        ]);
    
        sinon.stub(MedicalConditionMap, "toDTO").callsFake((condition) => condition);
    
        const medicalConditionServiceInstance = Container.get("MedicalConditionService") as IMedicalConditionService;
        const ctrl = new MedicalConditionController(medicalConditionServiceInstance);
    
        // Act
        await ctrl.getAllMedicalCondition(<Request>req, <Response>res, <NextFunction>next);
    
        // Assert
        sinon.assert.calledOnce(res.status as sinon.SinonStub);
        sinon.assert.calledWith(res.status as sinon.SinonStub, 200);
        sinon.assert.calledOnce(res.json as sinon.SinonSpy);
        sinon.assert.calledWith(res.json as sinon.SinonSpy, sinon.match(medicalConditionDTOList));
      });
    
      it("should handle not found error when no medical conditions exist", async () => {
        const req: Partial<Request> = {};
        const res: Partial<Response> = {
          status: sinon.stub().returnsThis(),
          send: sinon.spy(),
        };
        const next: Partial<NextFunction> = sinon.spy();
    
        const medicalConditionRepoInstance = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
        sinon.stub(medicalConditionRepoInstance, "findAll").resolves([]);
    
        const medicalConditionServiceInstance = Container.get("MedicalConditionService") as IMedicalConditionService;
        const ctrl = new MedicalConditionController(medicalConditionServiceInstance);
    
        // Act
        await ctrl.getAllMedicalCondition(<Request>req, <Response>res, <NextFunction>next);
    
        // Assert
        sinon.assert.calledOnce(res.status as sinon.SinonStub);
        sinon.assert.calledWith(res.status as sinon.SinonStub, 404);
        sinon.assert.calledOnce(res.send as sinon.SinonSpy);
      });

      it("should return a medical condition by id successfully", async () => {
        const medicalConditionDTO: IMedicalConditionDTO = {
            id: 'FB70.0',
            designation: 'Condition 1',
            description: 'Description 1',
            symptoms: ['Symptom A', 'Symptom B'],
        };

        const req: Partial<Request> = { body: { id: 'FB70.0' } };
        const res: Partial<Response> = {
            status: sinon.stub().returnsThis(),
            json: sinon.spy(),
        };
        const next: Partial<NextFunction> = sinon.spy();

        const medicalConditionRepoInstance = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
        sinon.stub(medicalConditionRepoInstance, "findByDomainId").resolves(medicalConditionDTO);

        sinon.stub(MedicalConditionMap, "toDTO").returns(medicalConditionDTO);

        const medicalConditionServiceInstance = Container.get("MedicalConditionService") as IMedicalConditionService;
        const ctrl = new MedicalConditionController(medicalConditionServiceInstance);

        // Act
        await ctrl.getMedicalConditionById(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.status as sinon.SinonStub);
        sinon.assert.calledWith(res.status as sinon.SinonStub, 200);
        sinon.assert.calledOnce(res.json as sinon.SinonSpy);
        sinon.assert.calledWith(res.json as sinon.SinonSpy, sinon.match(medicalConditionDTO));
    });

    it("should handle not found error when medical condition does not exist", async () => {
        const req: Partial<Request> = { body: { id: 'FB99.0' } };
        const res: Partial<Response> = {
            status: sinon.stub().returnsThis(),
            send: sinon.spy(),
        };
        const next: Partial<NextFunction> = sinon.spy();

        const medicalConditionRepoInstance = Container.get("MedicalConditionRepo") as IMedicalConditionRepo;
        sinon.stub(medicalConditionRepoInstance, "findByDomainId").resolves(null);

        const medicalConditionServiceInstance = Container.get("MedicalConditionService") as IMedicalConditionService;
        const ctrl = new MedicalConditionController(medicalConditionServiceInstance);

        // Act
        await ctrl.getMedicalConditionById(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.status as sinon.SinonStub);
        sinon.assert.calledWith(res.status as sinon.SinonStub, 404);
        sinon.assert.calledOnce(res.send as sinon.SinonSpy);
    });

    it("should handle invalid id error", async () => {
        const req: Partial<Request> = { body: { id: 123 } }; // Invalid type
        const res: Partial<Response> = {
            status: sinon.stub().returnsThis(),
            json: sinon.spy(),
        };
        const next: Partial<NextFunction> = sinon.spy();

        const medicalConditionServiceInstance = Container.get("MedicalConditionService") as IMedicalConditionService;
        const ctrl = new MedicalConditionController(medicalConditionServiceInstance);

        // Act
        await ctrl.getMedicalConditionById(<Request>req, <Response>res, <NextFunction>next);

        // Assert
        sinon.assert.calledOnce(res.status as sinon.SinonStub);
        sinon.assert.calledWith(res.status as sinon.SinonStub, 400);
        sinon.assert.calledOnce(res.json as sinon.SinonSpy);
        sinon.assert.calledWith(res.json as sinon.SinonSpy, sinon.match({ error: 'Invalid id provided.' }));
    });
});
