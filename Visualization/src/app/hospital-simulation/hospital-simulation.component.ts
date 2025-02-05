import { AfterViewInit, Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import * as THREE from "three";
import * as TWEEN from '@tweenjs/tween.js';
import layoutData from '../../../public/Loquitas.json'
import { GLTFLoader, OrbitControls } from 'three-stdlib';
import Wall from './simulation_classes/wall_template';
import Sprite from './simulation_classes/sprite_template';
import Camera from './simulation_classes/camera_template';
import Orientation from './simulation_classes/orientation';
import UserInterface from './simulation_classes/user_interface_template';
import Ground from './simulation_classes/ground_template';
import { ScheduleSlot } from '../domain/shedule-slot';
import { RoomService } from '../services/room.service';
import { RoomSchedule } from '../domain/room-schedule';
import { SideBarDoctorComponent } from '../components/staff/doctor/sidebar-doctor/side-bar-doctor.component';
import { Room } from '../domain/room';
import { UserInfo } from 'src/app/domain/user-info';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SurgeryAppointmentService } from '../services/surgery-appointment.service';
import { AppointmentWithoutStaff } from '../domain/appointment-without-staff';
import { OperationRequest } from '../domain/OperationRequest';
import { OperationRequestService } from '../services/operation-request.service';
import { PatientService } from '../services/patient.service';
import { Patient } from '../domain/Patient';
import { PatientWithId } from '../domain/patient-with-id';

type CameraParameters = {
  view: string;
  multipleViewsViewport: THREE.Vector4;
  target: THREE.Vector3;
  initialOrientation: Orientation;
  orientationMin: Orientation;
  orientationMax: Orientation;
  initialDistance: number;
  distanceMin: number;
  distanceMax: number;
  initialZoom: number;
  zoomMin: number;
  zoomMax: number;
  initialFov: number;
  near: number;
  far: number;
};

@Component({
  selector: 'app-hospital-simulation',
  standalone: true,
  imports: [SideBarDoctorComponent, CommonModule],
  templateUrl: './hospital-simulation.component.html',
  styleUrl: './hospital-simulation.component.css'
})

export class HospitalSimulationComponent implements AfterViewInit {

  @ViewChild('myCanvas') private canvasRef!: ElementRef<HTMLCanvasElement>;
  storedToken = localStorage.getItem('user');

  constructor(private roomService: RoomService, private appointmentService: SurgeryAppointmentService, private operationRequestService: OperationRequestService, private patientService: PatientService) {}

  private get canvas(): HTMLCanvasElement {
    return this.canvasRef.nativeElement;
  }

  spotlights : Map<string, THREE.SpotLight> = new Map<string, THREE.SpotLight>;

  overlayInfoVisible = false;
  private selectedRoom: string | null = null;
  appointment: AppointmentWithoutStaff | null = null;
  operationRequest: OperationRequest | null = null;
  patient: PatientWithId | null = null;
  roomIDs !: string[];
  rooms : Room[] = [];
  roomDetails: Room | null = null;
  roomSprites = new Map<string, Sprite>;
  roomLoadedPatients = new Map<string, boolean>;
  roomSchedule = new Map<string, Map<string, ScheduleSlot[]>>;
  scheduleDays !: string[];
  selectedDay : string = '';
  workDayTime : number = 0;

  private roomPositions = new Map<string, THREE.Vector3>;
  patientModel !: THREE.Group;

  private renderer!: THREE.WebGLRenderer | null;
  private camera!: THREE.PerspectiveCamera | null;
  private controls!: OrbitControls;
  private scene!: THREE.Scene | null;
  private animationFrameId: number = 0;

  view!: HTMLSelectElement;
  projection!: HTMLSelectElement;
  horizontal!: HTMLInputElement;
  vertical!: HTMLInputElement;
  distance!: HTMLInputElement;
  zoom!: HTMLInputElement;
  userInterface!: UserInterface  | null;

  fixedViewCamera!: Camera;
  changeCameraDistance!: boolean;
  changeCameraOrientation!: boolean;

  mousePosition!: THREE.Vector2;
  mouse !: THREE.Vector2;
  raycaster !: THREE.Raycaster;
  INTERSECTED !: THREE.Group | null;
  objectsToIntersect: THREE.Group[] = [];

  private tooltipCanvas !: HTMLCanvasElement;
  private tooltipContext !: CanvasRenderingContext2D;
  private tooltipTexture !: THREE.Texture;
  private tooltipSprite !: THREE.Sprite;

  private ground!: THREE.Mesh;
  private wall = new THREE.Group();
  private surgical_bed = new THREE.Group();
  private door = new THREE.Group();
  private layout = new THREE.Group();
  private scale = new THREE.Vector3(1.0, 1.0, 1.0);
  private bedScale = new THREE.Vector3(0.03, 0.03, 0.025);
  private patientScale = new THREE.Vector3(0.005, 0.005, 0.005);
  private doorScale = new THREE.Vector3(0.4, 1, 0.6);

  private createScene(): void {

    this.scene = new THREE.Scene();
    this.scene.background = new THREE.Color(0xa0a0a0);

    this.renderer = new THREE.WebGLRenderer({ canvas: this.canvas });
    this.renderer.autoClear = false;
    this.renderer.shadowMap.enabled = true;
    this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;
    this.renderer.setSize(this.canvas.clientWidth, this.canvas.clientHeight);
    this.renderer.setPixelRatio(devicePixelRatio);
    document.body.appendChild(this.renderer.domElement);

    this.camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 1, 10000);

    this.controls = new OrbitControls(this.camera, this.renderer.domElement); // Use active camera
    this.controls.minDistance = 10; // Minimum zoom-out distance
    this.controls.maxDistance = 100;

    this.controls.enableDamping = true; // Enable smooth transitions
    this.controls.dampingFactor = 0.25; // Adjust as needed
    this.controls.mouseButtons = {
      MIDDLE: THREE.MOUSE.DOLLY, // Middle-click dolly (zoom)
      RIGHT: THREE.MOUSE.ROTATE  // Right-click rotation
    };
    this.controls.zoomSpeed = 0.5;
    this.controls.update();

    // // Position the camera above the.layout, looking down
    this.camera.position.set(20, 30, 30); // Higher Y position to hover over.layout
    this.camera.lookAt(0, 0, 0); // Look down at the center of the scene

    this.userInterface = new UserInterface(this.scene, this.renderer, document, this.scheduleDays);

    this.raycaster = new THREE.Raycaster();
    this.mouse = new THREE.Vector2();

    this.setupEventListeners();

    this.tooltipCanvas = document.createElement('canvas');
    const context = this.tooltipCanvas.getContext('2d');
    if (context === null) {
      throw new Error("Failed to get 2D context for tooltipCanvas");
    }
    this.tooltipContext = context;

    this.tooltipCanvas.width = 200;
    this.tooltipCanvas.height = 80;
    this.tooltipContext.font = "Bold 15px Arial";
    this.tooltipTexture = new THREE.Texture(this.tooltipCanvas);
    this.tooltipTexture.needsUpdate = true;

    var spriteMaterial = new THREE.SpriteMaterial({
      map: this.tooltipTexture,
      transparent: true,
    });

    this.tooltipSprite = new THREE.Sprite(spriteMaterial);
    this.tooltipSprite.visible = false;
    this.tooltipSprite.scale.set(1.5, 0.7, 1);
    this.layout.add(this.tooltipSprite);
    this.scene.add(this.tooltipSprite);
  }


  initializeFixedCamera() {
    const cameraData: CameraParameters = {
      view: "fixed",
      multipleViewsViewport: new THREE.Vector4(0.0, 0.0, 1.0, 1.0), // Viewport position and size: fraction of window width and window height
      target: new THREE.Vector3(0.0, 0.0, 0.0), // Target position
      initialOrientation: new Orientation(135.0, -45.0), // Horizontal and vertical orientation and associated limits (expressed in degrees)
      orientationMin: new Orientation(-180.0, -90.0),
      orientationMax: new Orientation(180.0, 0.0),
      initialDistance: 8.0, // Distance to the target and associated limits
      distanceMin: 4.0,
      distanceMax: 16.0,
      initialZoom: 1.0, // Zoom factor and associated limits
      zoomMin: 0.5,
      zoomMax: 2.0,
      initialFov: 45.0, // Field-of-view (expressed in degrees)
      near: 0.01, // Front clipping plane
      far: 100.0 // Back clipping plane
    };

    this.fixedViewCamera = new Camera(cameraData, this.canvas.width, this.canvas.height);

    // Set the active view camera (fixed view)
    this.setActiveViewCamera(this.fixedViewCamera);
  }

  setActiveViewCamera(camera: any) {
    this.fixedViewCamera = camera;
    this.horizontal.min = this.fixedViewCamera.orientationMin.h.toFixed(0);
    this.horizontal.max = this.fixedViewCamera.orientationMax.h.toFixed(0);
    this.vertical.min = this.fixedViewCamera.orientationMin.v.toFixed(0);
    this.vertical.max = this.fixedViewCamera.orientationMax.v.toFixed(0);
    this.distance.min = this.fixedViewCamera.distanceMin.toFixed(1);
    this.distance.max = this.fixedViewCamera.distanceMax.toFixed(1);
    this.zoom.min = this.fixedViewCamera.zoomMin.toFixed(1);
    this.zoom.max = this.fixedViewCamera.zoomMax.toFixed(1);
  }

  raycasterIntersectedObjectTooltip(intersects: any) {
    if (intersects.length > 0) {
        let intersectedObject = intersects[0].object;

        while (intersectedObject.parent && !intersectedObject.parent.userData.isParentGroup) {
            intersectedObject = intersectedObject.parent;
        }

        if (intersectedObject.parent) {
          intersectedObject = intersectedObject.parent;
        }

        if (intersectedObject !== this.INTERSECTED) {
            this.INTERSECTED = intersectedObject;

            console.log(this.INTERSECTED!.name);

            if (this.INTERSECTED && this.INTERSECTED.name.startsWith('Patient')) {
                let message = this.getAppoitmentInfo(this.INTERSECTED.name);
                if (message) {
                  this.showTooltip(message, intersectedObject.getWorldPosition(new THREE.Vector3()));
                }
            }
        }
    } else {

            this.hideTooltip();
            this.INTERSECTED = null;

    }
}

  showTooltip(message: string, position: THREE.Vector3) {
    this.tooltipContext.clearRect(0, 0, this.tooltipCanvas.width, this.tooltipCanvas.height);
    this.tooltipContext.fillStyle = "rgba(255, 255, 255, 0.9)";
    this.tooltipContext.fillRect(0, 0, 200, 250);
    this.tooltipContext.fillStyle = "black";

    const lines = message.split("\n");

    const lineHeight = 20;
    let yPos = 30;

    for (let line of lines) {
        this.tooltipContext.fillText(line, 10, yPos);
        yPos += lineHeight;
    }

    this.tooltipTexture.needsUpdate = true;

    this.tooltipSprite.position.copy(position);
    this.tooltipSprite.position.y += 4.5;
    this.tooltipSprite.visible = true;
  }

  hideTooltip() {
    this.tooltipSprite.visible = false;
  }

  getAppoitmentInfo(name : string) : string | null{
    var roomID = name.split("-")[1];
    var appointment = this.getRoomAppointment(roomID);

    if (appointment){
      var operation = appointment.name === '' ? "Occupied" : appointment.name;
      console.log(`${operation}\nStart: ${appointment.startTime}\nEnd: ${appointment.endTime}`)
      return `${operation}\nStart: ${appointment.startTime}\nEnd: ${appointment.endTime}`;
    }
    return null;
  }


  getRoomAppointment(roomID: string): ScheduleSlot | null {
    if (this.roomSchedule.has(roomID)) {
        const roomScheduleMap = this.roomSchedule.get(roomID)!;

        if (roomScheduleMap.has(this.selectedDay)) {
            const scheduleSlots = roomScheduleMap.get(this.selectedDay)!;

            for (let schedule of scheduleSlots) {
                const startTime = this.convertTimeToDecimalHours(schedule.startTime);
                const endTime = this.convertTimeToDecimalHours(schedule.endTime);

                if (this.workDayTime >= startTime && this.workDayTime < endTime) {
                    return schedule;
                }
            }
        }
    }

    return null;
}

private createModel(modelUrl: string, scale: any): Promise<THREE.Group> {
  let model = new THREE.Group();
  const loader = new GLTFLoader();
  return new Promise((resolve, reject) => {
    loader.load(
      modelUrl,
      (gltf) => {
        model = gltf.scene;
        model.scale.copy(scale);

        // Traverse through all objects in the model's scene
        model.traverse((child) => {
          if (child instanceof THREE.Mesh) {
            // Ensure mesh casts and receives shadows
            child.castShadow = true;
            child.receiveShadow = true;
          }
        });

        resolve(model);
      },
      (xhr) => {
        console.log((xhr.loaded / xhr.total) * 100 + '% loaded');
      },
      (error) => {
        console.error('An error occurred while loading the model:', error);
        reject(error);
      }
    );
  });
}

  private async loadLayout(data: any): Promise<void> {
    this.ground = new Ground(data.groundTextureUrl, data.size).object;
    this.ground.position.y = -1.265;
    this.layout.add(this.ground);

    this.wall = new Wall(data.wallTextureUrl).object;

    this.surgical_bed = await this.createModel(data.bedModel, this.bedScale);
    this.patientModel = await this.createModel(data.patientModel, this.patientScale);
    this.door = await this.createModel(data.doorModelUrl, this.doorScale);

    const textureLoader = new THREE.TextureLoader();
    this.door.traverse((child: any) => {
      if (child.isMesh) {
        const material = child.material as THREE.MeshStandardMaterial;
        material.map = textureLoader.load(data.doorTextureUrl);
        material.color.set(0xD7EBFA);
        material.needsUpdate = true;
      }
    });

    // Build the maze
    for (let i = 0; i <= data.size.width; i++) {
      for (let j = 0; j <= data.size.height; j++) {

        // West wall
        if (data.layout[j][i] === 1 || data.layout[j][i] === 3) {
          const wallObject = this.wall.clone();
          wallObject.rotateY(Math.PI / 2);
          wallObject.position.set(i - data.size.width / 2, 0.5, j - data.size.height / 2 - 0.5);
          wallObject.name = "Wall";
          this.layout.add(wallObject);
          this.objectsToIntersect.push(wallObject);
        }

        // North wall
        if (data.layout[j][i] === 2 || data.layout[j][i] === 3) {
          const wallObject = this.wall.clone();
          wallObject.position.set(i - data.size.width / 2 + 0.5, 0.5, j - data.size.height / 2);
          wallObject.name = "Wall";
          this.layout.add(wallObject);
          this.objectsToIntersect.push(wallObject);
        }

        //North Surgical bed
        if (data.layout[j][i] === 4) {
          const bedObject = this.surgical_bed.clone();
          bedObject.position.set(i - data.size.width / 2, - 0.8, j - data.size.height / 2 - 0.5);
          bedObject.scale.set(0.03, 0.03, 0.025);
          bedObject.name = "Surgical_Bed";
          bedObject.rotation.y = Math.PI / 2;
          bedObject.castShadow = true;
          bedObject.receiveShadow = true;
          this.layout.add(bedObject);
          this.objectsToIntersect.push(bedObject);
        }

        //North Door
        if (data.layout[j][i] === 5) {
          const doorObject = this.door.clone();
          doorObject.position.set(i - data.size.width / 2, -1.25, j - data.size.height / 2 - 0.4);
          doorObject.position.z -= 0.55;
          doorObject.scale.set(0.7, 1, 0.6);
          doorObject.rotation.y = Math.PI;
          doorObject.name = "North Door";
          doorObject.castShadow = true;
          doorObject.receiveShadow = true;
          this.layout.add(doorObject);
          this.objectsToIntersect.push(doorObject);
        }

        //South Surgical bed
        if (data.layout[j][i] === 7) {
          const bedObject = this.surgical_bed.clone();
          bedObject.position.set(i - data.size.width / 2, -0.8, j - data.size.height / 2 - 0.5);
          bedObject.scale.set(0.03, 0.03, 0.025);
          bedObject.name = "Surgical_Bed";
          bedObject.rotation.y = - Math.PI / 2;
          bedObject.castShadow = true;
          bedObject.receiveShadow = true;
          this.layout.add(bedObject);
          this.objectsToIntersect.push(bedObject);
        }

        //South Door
        if (data.layout[j][i] === 8) {
          const doorObject = this.door.clone();
          doorObject.position.set(i - data.size.width / 2, -1.25, j - data.size.height / 2 - 0.4);
          doorObject.position.z -= 0.55;
          doorObject.scale.set(0.7, 1, 0.6);
          doorObject.name = "South Door";
          doorObject.rotation.y = Math.PI;
          doorObject.castShadow = true;
          doorObject.receiveShadow = true;
          this.layout.add(doorObject);
          this.objectsToIntersect.push(doorObject);

        }


        //West Door
        if (data.layout[j][i] === 9) {
          const doorObject = this.door.clone();
          doorObject.position.set(i - data.size.width / 2, -1.25, j - data.size.height / 2 - 0.4);
          doorObject.position.x -= 1.05;
          doorObject.position.z += 0.4;
          doorObject.scale.set(0.7, 1, 0.6);
          doorObject.rotation.y = Math.PI / 2;
          doorObject.name = "West Door";
          doorObject.castShadow = true;
          doorObject.receiveShadow = true;
          this.layout.add(doorObject);
          this.objectsToIntersect.push(doorObject);

        }

        //East Door
        if (data.layout[j][i] === 10) {
          const doorObject = this.door.clone();
          doorObject.position.set(i - data.size.width / 2, -1.25, j - data.size.height / 2 - 0.4);
          doorObject.position.x += 0.95;
          doorObject.position.z += 0.4;
          doorObject.scale.set(0.7, 1, 0.6);
          doorObject.rotation.y = Math.PI / 2;
          doorObject.name = "East Door";
          doorObject.castShadow = true;
          doorObject.receiveShadow = true;
          this.layout.add(doorObject);
          this.objectsToIntersect.push(doorObject);

        }

      }
    }

  let index = 0;

    for (let i = 0; i <= data.size.width; i++) {
        for (let j = 0; j <= data.size.height; j++) {
            if (data.layout[j][i] === 4 || data.layout[j][i] === 7) {
                if (index < this.roomIDs.length) {
                    const position = this.cellToCartesian(data, [j, i]);
                    this.roomPositions.set(this.roomIDs[index], position);
                    this.roomLoadedPatients.set(this.roomIDs[index], false);

                    const sprite = new Sprite("   " + this.roomIDs[index] + "   ", true);
                    sprite.object.position.copy(position);
                    sprite.object.position.y += 2.5;
                    sprite.object.position.x += 0.5;
                    this.roomSprites.set(this.roomIDs[index], sprite);
                    this.layout.add(sprite.object);

                    // Add spotlight for the room
                    const spotlight = new THREE.SpotLight(0xffffff, 0);
                    spotlight.position.set(position.x, position.y+1.5 , position.z);
                    spotlight.target.position.set(position.x, position.y, position.z);
                    spotlight.castShadow = true;
                    this.scene!.add(spotlight);
                    this.scene!.add(spotlight.target);
                    this.spotlights.set(this.roomIDs[index], spotlight);

                    ++index;
                } else {
                    console.warn(`Not enough room IDs to assign to sprites. Current index: ${index}, roomIDs length: ${this.roomIDs.length}`);
                    break;
                }
            }
        }
    }

    this.layout.scale.set(this.scale.x, this.scale.y, this.scale.z);
    this.scene!.add(this.layout);
}

  private setupEventListeners() {
    this.renderer!.domElement.addEventListener("wheel", (event) => this.mouseWheel(event));
    document.addEventListener("mousemove", event => {
      const rect = this.canvas.getBoundingClientRect();

      this.mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
      this.mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;
  });
    document.addEventListener('click', (event) => this.onDocumentMouseClick(event));
    document.addEventListener('keydown', (event) => {
      if (event.key.toLowerCase() === "i") {
          this.toggleRoomInfoOverlay();
      }  else {
        console.warn("Nenhum quarto selecionado. Pressiona num quarto antes.");
      }
    });
  }


  onDocumentMouseClick(event: MouseEvent) {
    const rect = this.canvas.getBoundingClientRect();
    this.mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
    this.mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;

    this.spotlights.forEach(spotlight => {
      spotlight.intensity = 0;
    });

    if (this.INTERSECTED?.name.startsWith('Patient')) {
        const targetPosition = this.INTERSECTED?.getWorldPosition(new THREE.Vector3()).normalize();
        //const targetPosition = this.INTERSECTED?.position;
        this.selectedRoom = this.INTERSECTED?.name;
        this.moveCameraToRoomCenter(targetPosition, this.selectedRoom);
        this.updateRoomInfoOverlay(this.selectedRoom);
    }
}

moveCameraToRoomCenter(targetPosition: THREE.Vector3 | undefined, selectedRoom: string) {
  if (!targetPosition) return;

  const roomId = selectedRoom.replace("Patient-", "");
  console.log("RoomId:", roomId);
  const spotlight = this.spotlights.get(roomId); // Retrieve the spotlight from the map
  if (spotlight) {
      spotlight.intensity = 30; // Set the desired intensity
  }

  // Define the room center based on the target position
  const roomCenter = new THREE.Vector3(
    spotlight?.target.position.x,
    spotlight?.target.position.y,
    spotlight?.target.position.z
  );

  // Clone the camera's starting position
  const startPosition = this.camera!.position.clone();

  // Animate the camera movement
  new TWEEN.Tween(startPosition)
    .to(
      {
        x: roomCenter.x,
        y: startPosition.y,
        z: roomCenter.z,
      },
      2800
    )
    .easing(TWEEN.Easing.Quadratic.InOut)
    .onUpdate(() => {
      this.camera!.position.copy(startPosition);
    })
    .onComplete(() => {
      // Optional: Add logic to remove spotlight after animation if needed
      console.log('Camera has reached the room center. Spotlight added.');
    })
    .start();
}

toggleRoomInfoOverlay() {
  this.overlayInfoVisible = !this.overlayInfoVisible;
  console.log("Overlay:", this.overlayInfoVisible ? "Visible" : "Ocult");

  if (this.overlayInfoVisible && this.selectedRoom) {
      const roomId = this.selectedRoom.replace("Patient-", "");
      console.log("RoomId:", roomId);

      this.roomDetails = this.rooms.find(r => r.roomNumber === roomId) || null;

      if (this.roomDetails) {
          console.log("Room information updated:", this.roomDetails);
      } else {
          console.error("The Room with RoomId was not found:", roomId);
      }
  } else {
      this.roomDetails = null;
  }
}

updateRoomInfoOverlay(selectedRoom: string) {
  console.log("Updated overlay for the room:", selectedRoom);

  const roomId = selectedRoom.replace("Patient-", "");
  console.log("RoomId:", roomId);
  const spotlight = this.spotlights.get(roomId); // Retrieve the spotlight from the map
  if (spotlight) {
      spotlight.intensity = 10; // Set the desired intensity
  }
  this.roomDetails = this.rooms.find(r => r.roomNumber === roomId) || null;

  if (this.roomDetails) {
    if (this.roomDetails.maintenanceSlots && this.roomDetails.maintenanceSlots.length > 0) {
      const currentSlot = this.roomDetails.maintenanceSlots.find(slot => {
        const slotHour = parseInt(slot.startTime.split(':')[0], 10);
        return slotHour === this.workDayTime;
      });

      if (currentSlot) {
        const startDate = currentSlot.startDate.split(' ')[0];
        const endDate = currentSlot.endDate.split(' ')[0];
        const startTime = currentSlot.startTime.split(':').slice(0, 2).join(':');
        const endTime = currentSlot.endTime.split(':').slice(0, 2).join(':');

        this.appointmentService.getAppointmentFromRoom(this.roomDetails.roomNumber, startTime, endTime, startDate, endDate
        ).subscribe({
          next: (data) => {
            this.appointment = data;

            if(this.appointment != null){
              this.operationRequestService.getOperationRequestById(this.appointment!.operationRequestId
              ).subscribe({
                next: (data) => {
                  this.operationRequest = data;

                  if(this.operationRequest != null){
                    this.patientService.getPatientById(this.operationRequest!.patientId
                    ).subscribe({
                      next: (data) => {
                        this.patient = data;
                      },
                      error: (error) => {
                        console.log("No patient:", error);
                        this.patient = null;
                      }
                    });
                  }
                },
                error: (error) => {
                  console.log("There are no appointment at that time:", error);
                  this.operationRequest = null;
                }
              });
            }
          },
          error: (error) => {
            console.log("There are no appointment at that time");
            this.appointment = null;
          }
        });
      } else {
        console.log("No matching slot found for the current time:", this.workDayTime);
      }
    } else {
      console.log("No maintenance slots available for the room.");
    }

    this.roomDetails.maintenanceSlots = this.roomDetails?.maintenanceSlots.map(slot => ({
      ...slot,
      startDate: slot.startDate.split(' ')[0],
      startTime: slot.startTime.split(':').slice(0, 2).join(':'),
      endDate: slot.endDate.split(' ')[0],
      endTime: slot.endTime.split(':').slice(0, 2).join(':')
    }));

    console.log("Room information updated:", this.roomDetails);
  } else {
    console.error("The Room with RoomId was not found:", roomId);
  }
}




  mouseWheel(event: WheelEvent) {
    // Prevent the mouse wheel from scrolling the document's content
    event.preventDefault();
    // Store current mouse position in window coordinates (mouse coordinate system: origin in the top-left corner; window coordinate system: origin in the bottom-left corner)
    this.mousePosition = new THREE.Vector2(event.clientX, window.innerHeight - event.clientY - 1);
    // Select the camera whose view is being pointed
    const cameraView = this.getPointedViewport(this.mousePosition);
    if (cameraView != "none") { // One of the remaining cameras selected
      const cameraIndex = ["fixed", "first-person", "third-person", "top"].indexOf(cameraView);
      this.view.options.selectedIndex = cameraIndex;
      const activeViewCamera = [this.fixedViewCamera][cameraIndex];
      activeViewCamera.updateZoom(-0.001 * event.deltaY);
      this.setActiveViewCamera(activeViewCamera);
    }
  }

  pointerIsOverViewport(pointer: any, viewport: any) {
    return (
      pointer.x >= viewport.x &&
      pointer.x < viewport.x + viewport.width &&
      pointer.y >= viewport.y &&
      pointer.y < viewport.y + viewport.height);
  }

  getPointedViewport(pointer: any) {
    let viewport;
    const cameras = [this.fixedViewCamera];

    for (const camera of cameras) {
        if (!camera || typeof camera.getViewport !== "function") {
            console.warn("Camera or getViewport method is not defined:", camera);
            continue;
        }
        viewport = camera.getViewport();
        if (this.pointerIsOverViewport(pointer, viewport)) {
            return camera.view;
        }
    }
    return "none";
}

  cellToCartesian(data: any, cell: [number, number]): THREE.Vector3 {
    const [row, col] = cell;
    return new THREE.Vector3(
      (col - data.size!.width / 2.0 + 0.5) * this.scale.x,
      0.0,
      (row - data.size!.height / 2.0 + 0.5) * this.scale.z
    );
  }

  cartesianToCell(data: any, position : THREE.Vector3) {
    return [
        Math.floor(position.z / this.scale.z + data.size!.height / 2.0),
        Math.floor(position.x / this.scale.x + data.size!.width / 2.0)
    ];
}

  checkOccupancy() {
    for (let [roomName, roomScheduleMap] of this.roomSchedule) {
        let isOccupied = false;


        if (roomScheduleMap.has(this.selectedDay)) {
            const scheduleSlots = roomScheduleMap.get(this.selectedDay)!;

            for (let schedule of scheduleSlots) {
                const startTime = this.convertTimeToDecimalHours(schedule.startTime);
                const endTime = this.convertTimeToDecimalHours(schedule.endTime);

                if (this.workDayTime >= startTime && this.workDayTime < endTime) {
                    isOccupied = true;
                    break;
                }
            }
        }

        if (isOccupied) {
            if (!this.roomLoadedPatients.get(roomName)) {
                this.loadPatient(roomName);
                this.updateSprite(roomName, false);
            }
        }
        else if (this.roomLoadedPatients.get(roomName) == true) {
                this.removePatient(roomName);
                this.updateSprite(roomName, true);
        }
    }
  }

  convertTimeToDecimalHours(timeString: string): number {
    const [hours, minutes] = timeString.split(':').map(Number);
    return hours + minutes / 60;
  }

  loadPatient(room: string) {
      const RoomPosition = this.roomPositions.get(room);
      if (!RoomPosition) {
          console.error(`Room position not found for room: ${room}`);
          return;
      }

      const patientGroup = new THREE.Group();


      const cell = this.cartesianToCell(layoutData, RoomPosition);
      if (cell) {
          const [j, i] = cell;
          const cellType = layoutData.layout[j][i];

          patientGroup.name = `Patient-${room}`;
          patientGroup.add(this.patientModel.clone());
          patientGroup.position.copy(RoomPosition);
          patientGroup.position.y -= 0.5;
          patientGroup.position.x -= 0.5;
          patientGroup.scale.set(1.5, 1.5, 1.5);
          patientGroup.userData['isParentGroup'] = true;


          if (cellType === 4) {
            patientGroup.rotation.y = - Math.PI;
            patientGroup.position.z -= 0.7;
          } else if (cellType === 7) {
            patientGroup.rotation.y =  Math.PI * 2;
            patientGroup.position.z -= 1.1;
          }
      } else {
          console.warn(`Could not determine cell for position: ${RoomPosition}`);
      }

      this.objectsToIntersect.push(patientGroup);
      this.layout.add(patientGroup);

      this.roomLoadedPatients.set(room, true);
  }

    removePatient(room: string) {
      const patientName = `Patient-${room}`;
      const patient = this.layout.getObjectByName(patientName);

      if (patient) {
          this.layout.remove(patient);
          this.objectsToIntersect = this.objectsToIntersect.filter((obj) => obj !== patient);
          this.roomLoadedPatients.set(room, false);
      } else {
          console.warn(`No patient found with name: ${patientName}`);
      }
  }

  updateSprite(room: string, occupied: boolean) {
      let sprite = this.roomSprites.get(room);

      if (sprite) {
          sprite.available = occupied;
          sprite.updateColor();
      }else {
        console.warn(`No sprite associated with room id: ${room}`);
      }
  }


  startAnimationLoop() {
    const animate = () => {

      this.raycaster.setFromCamera(this.mouse, this.camera!);

      const intersects = this.raycaster.intersectObjects(this.objectsToIntersect);

      this.raycasterIntersectedObjectTooltip(intersects);

      this.workDayTime = this.userInterface!.timeSettings.hours;
      this.selectedDay = this.userInterface!.selectedDay.day;

      this.checkOccupancy();

      TWEEN.update();

      this.animationFrameId = requestAnimationFrame(animate);
        this.renderer!.render(this.scene!, this.camera!);
        this.controls.update();
    };

    animate();
  }

  ngOnDestroy(): void {
    if (this.animationFrameId) {
      cancelAnimationFrame(this.animationFrameId);
    }
    this.cleanup();
  }


  cleanup() {
    this.renderer!.dispose();
    this.scene = null;
    this.camera = null;

    const canvas = document.getElementById('myCanvas');
    if (canvas) {
        canvas.remove();
    }

    const guiContainer = this.userInterface!.gui?.domElement;
    if (guiContainer && guiContainer.parentNode) {
        guiContainer.parentNode.removeChild(guiContainer);
    }
    this.userInterface = null;
}

  loadDataFromBackend(): void {
    this.roomService.getRoomsSchedule().subscribe((scheduledBackend: RoomSchedule[]) => {

      this.roomIDs = scheduledBackend.map((roomSchedule) => roomSchedule.roomNumber);
      this.roomSchedule = new Map<string, Map<string, ScheduleSlot[]>>();
      this.scheduleDays = [];

      for (let room of scheduledBackend) {
        const scheduleMap = new Map<string, ScheduleSlot[]>();

        room.schedule.forEach((slot) => {
          if (!scheduleMap.has(slot.startDate.split(" ")[0])) {
            scheduleMap.set(slot.startDate.split(" ")[0], []);
          }
          scheduleMap.get(slot.startDate.split(" ")[0])!.push(slot);
        });

        this.roomSchedule.set(room.roomNumber, scheduleMap);
      }

      this.roomSchedule.forEach((scheduleMap) => {
        scheduleMap.forEach((slots, date) => {
          if (!this.scheduleDays.includes(date)) {
            this.scheduleDays.push(date.split(" ")[0]);
          }
        });
      });

      this.scheduleDays.sort((a, b) => new Date(b).getTime() - new Date(a).getTime());

      this.createScene();
      this.loadLayout(layoutData);
      document.body.appendChild(this.renderer!.domElement);
      this.startAnimationLoop();
    }, (error) => {
      console.error("Error loading data from backend:", error);
    });
  }

  loadRooms() {
    this.roomService.getAllRooms().subscribe({
        next: (roomList: Room[]) => {
            this.rooms = roomList;
        },
        error: (err) => {
            console.error("Error loading room data:", err);
        }
    });
}

  async ngAfterViewInit(): Promise<void> {
    this.loadDataFromBackend();
    this.loadRooms();
  }

  @HostListener('window:resize', ['$event'])
  onResize(): void {
      this.resize();
  }

  private resize(): void {
      const width = this.canvas.clientWidth;
      const height = this.canvas.clientHeight;

      // Update renderer size
      this.renderer!.setSize(width, height);

      // Update camera aspect ratio and projection matrix
      this.camera!.aspect = width / height;
      this.camera!.updateProjectionMatrix();
  }

}
