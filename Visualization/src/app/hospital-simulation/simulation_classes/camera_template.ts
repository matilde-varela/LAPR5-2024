import * as THREE from "three";
import Orientation from "./orientation";

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

export default class Camera {
  view!: string;
  multipleViewsViewport!: THREE.Vector4;
  target: THREE.Vector3;
  initialOrientation!: Orientation;
  orientationMin!: Orientation;
  orientationMax!: Orientation;
  initialDistance!: number;
  distanceMin!: number;
  distanceMax!: number;
  initialZoom!: number;
  zoomMin!: number;
  zoomMax!: number;
  initialFov!: number;
  near!: number;
  far!: number;

  viewport: THREE.Vector4;
  initialHalfSize: number;
  userDirection: number;

  perspective: THREE.PerspectiveCamera;
  orthographic: THREE.OrthographicCamera;

  orientation!: Orientation;
  distance!: number;
  zoom!: number;
  aspectRatio!: number;

  windowWidth!: number;
  windowHeight!: number;
  projection!: string;
  object!: THREE.Camera;

  constructor(parameters: CameraParameters, windowWidth: number, windowHeight: number) {
    Object.assign(this, parameters);
    this.viewport = this.multipleViewsViewport.clone();
    this.target = new THREE.Vector3();

    this.initialHalfSize = Math.tan(THREE.MathUtils.degToRad(this.initialFov / 2)) * this.initialDistance;
    this.userDirection = 0.0;

    this.perspective = new THREE.PerspectiveCamera();
    this.orthographic = new THREE.OrthographicCamera();

    this.setWindowSize(windowWidth, windowHeight);
    this.initialize();
  }

  setViewingParameters(): void {
    const orientation = new Orientation(this.orientation.h + this.userDirection, this.orientation.v);
    const cosH = Math.cos(THREE.MathUtils.degToRad(orientation.h));
    const sinH = Math.sin(THREE.MathUtils.degToRad(orientation.h));
    const cosV = Math.cos(THREE.MathUtils.degToRad(orientation.v));
    const sinV = Math.sin(THREE.MathUtils.degToRad(orientation.v));

    let positionX = this.target.x;
    let positionY = this.target.y;
    let positionZ = this.target.z;

    if (this.view !== "first-person") {
      positionX -= this.distance * sinH * cosV;
      positionY -= this.distance * sinV;
      positionZ -= this.distance * cosH * cosV;
    }
    this.perspective.position.set(positionX, positionY, positionZ);
    this.orthographic.position.set(positionX, positionY, positionZ);

    const upX = -sinH * sinV;
    const upY = cosV;
    const upZ = -cosH * sinV;
    this.perspective.up.set(upX, upY, upZ);
    this.orthographic.up.set(upX, upY, upZ);

    const target = this.target.clone();
    if (this.view === "first-person") {
      target.x += sinH * cosV;
      target.y += sinV;
      target.z += cosH * cosV;
    }
    this.perspective.lookAt(target);
    this.orthographic.lookAt(target);
  }

  setProjectionParameters(): void {
    let fov: number, left: number, right: number, top: number, bottom: number;

    if (this.aspectRatio < 1.0) {
      fov = 2.0 * THREE.MathUtils.radToDeg(Math.atan(Math.tan(THREE.MathUtils.degToRad(this.initialFov / 2)) / this.aspectRatio));
      right = this.initialHalfSize;
      left = -right;
      top = right / this.aspectRatio;
      bottom = -top;
    } else {
      fov = this.initialFov;
      top = this.initialHalfSize;
      bottom = -top;
      right = top * this.aspectRatio;
      left = -right;
    }

    this.perspective.fov = fov;
    this.perspective.aspect = this.aspectRatio;
    this.perspective.near = this.near;
    this.perspective.far = this.far;
    this.perspective.zoom = this.zoom;
    this.perspective.updateProjectionMatrix();

    this.orthographic.left = left;
    this.orthographic.right = right;
    this.orthographic.top = top;
    this.orthographic.bottom = bottom;
    this.orthographic.near = this.near;
    this.orthographic.far = this.far;
    this.orthographic.zoom = this.zoom;
    this.orthographic.updateProjectionMatrix();
  }

  setActiveProjection(projection: string): void {
    this.projection = projection;
    this.object = projection !== "orthographic" ? this.perspective : this.orthographic;
  }

  initialize(): void {
    this.orientation = this.initialOrientation.clone();
    this.distance = this.initialDistance;
    this.zoom = this.initialZoom;

    this.setViewingParameters();
    this.setProjectionParameters();
    this.setActiveProjection(this.view !== "mini-map" ? "perspective" : "orthographic");
  }

  getViewport(): THREE.Vector4 {
    const windowMinSize = Math.min(this.windowWidth, this.windowHeight);
    let x: number, y: number;
    let width = this.viewport.width;
    let height = this.viewport.height;

    if (this.view !== "mini-map") {
      x = this.viewport.x * (1.0 - this.viewport.width);
      y = this.viewport.y * (1.0 - this.viewport.height);

      if (this.windowWidth < this.windowHeight) {
        x *= windowMinSize;
        y *= this.windowHeight;
        width *= windowMinSize;
        height *= this.windowHeight;
      } else {
        x *= this.windowWidth;
        y *= windowMinSize;
        width *= this.windowWidth;
        height *= windowMinSize;
      }
    } else {
      width *= windowMinSize;
      height *= windowMinSize;
      x = this.viewport.x * (this.windowWidth - width);
      y = this.viewport.y * (this.windowHeight - height);
    }
    return new THREE.Vector4(x, y, width, height);
  }

  setViewport(multipleViews: boolean): void {
    this.viewport = multipleViews ? this.multipleViewsViewport.clone() : new THREE.Vector4(0.0, 0.0, 1.0, 1.0);
    const viewport = this.getViewport();
    this.aspectRatio = viewport.width / viewport.height;
    this.setProjectionParameters();
  }

  setWindowSize(windowWidth: number, windowHeight: number): void {
    this.windowWidth = windowWidth;
    this.windowHeight = windowHeight;
    const viewport = this.getViewport();
    this.aspectRatio = viewport.width / viewport.height;
  }

  updateWindowSize(windowWidth: number, windowHeight: number): void {
    this.setWindowSize(windowWidth, windowHeight);
    this.setProjectionParameters();
  }

  setTarget(target: THREE.Vector3): void {
    this.target.copy(target);
    this.setViewingParameters();
  }

  updateTarget(targetIncrement: THREE.Vector3): void {
    this.setTarget(this.target.add(targetIncrement));
  }

  setOrientation(orientation: Orientation): void {
    this.orientation.copy(orientation).clamp(this.orientationMin, this.orientationMax);
    this.setViewingParameters();
  }

  updateOrientation(orientationIncrement: Orientation): void {
    this.setOrientation(this.orientation.add(orientationIncrement));
  }

  setDistance(distance: number): void {
    this.distance = THREE.MathUtils.clamp(distance, this.distanceMin, this.distanceMax);
    this.setViewingParameters();
  }

  updateDistance(distanceIncrement: number): void {
    this.setDistance(this.distance + distanceIncrement);
  }

  setZoom(zoom: number): void {
    this.zoom = THREE.MathUtils.clamp(zoom, this.zoomMin, this.zoomMax);
    this.perspective.zoom = this.zoom;
    this.perspective.updateProjectionMatrix();
    this.orthographic.zoom = this.zoom;
    this.orthographic.updateProjectionMatrix();
  }

  updateZoom(zoomIncrement: number): void {
    this.setZoom(this.zoom + zoomIncrement);
  }
}
