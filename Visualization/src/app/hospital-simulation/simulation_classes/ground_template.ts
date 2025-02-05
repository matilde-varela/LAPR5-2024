import * as THREE from "three";

export default class Ground {
  public object: THREE.Mesh;

  constructor(textureUrl : string, size : THREE.Vector2) {

    // Load the texture
    const texture = new THREE.TextureLoader().load(textureUrl);
    texture.colorSpace = THREE.SRGBColorSpace;
    texture.wrapS = THREE.RepeatWrapping;
    texture.wrapT = THREE.RepeatWrapping;
    texture.repeat.set(size.width, size.height);
    texture.magFilter = THREE.LinearFilter;
    texture.minFilter = THREE.LinearMipmapLinearFilter;

    // Create the plane geometry
    const geometry = new THREE.PlaneGeometry(size.width, size.height);

    // Create the material with the texture
    const material = new THREE.MeshPhongMaterial({ color: 0x90adbf, map: texture, side: THREE.DoubleSide });

    // Create the mesh
    this.object = new THREE.Mesh(geometry, material);
    this.object.rotation.x = -Math.PI / 2.0;
    this.object.position.y = 0;

    // Configure shadows
    this.object.castShadow = false;
    this.object.receiveShadow = true;
  }
}
