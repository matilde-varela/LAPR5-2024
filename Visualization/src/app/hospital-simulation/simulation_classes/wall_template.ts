import * as THREE from "three";

export default class Wall {
    object: THREE.Group;

    constructor(textureUrl: string) {
        // Load texture
        const texture = new THREE.TextureLoader().load(textureUrl);
        texture.colorSpace = THREE.SRGBColorSpace;
        texture.magFilter = THREE.LinearFilter;
        texture.minFilter = THREE.LinearMipmapLinearFilter;
        texture.wrapS = THREE.RepeatWrapping;
        texture.wrapT = THREE.RepeatWrapping;
        texture.repeat.set(1, 1);

        // Create a group to hold the wall
        this.object = new THREE.Group();

        // Create wall geometry and material
        const geometry = new THREE.BoxGeometry(1, 3.5, 0.05);
        const material = new THREE.MeshPhongMaterial({ color: 0xffffff, map: texture });
        const wall = new THREE.Mesh(geometry, material);

        // Enable shadows
        wall.castShadow = true;
        wall.receiveShadow = true;

        // Add wall to the group
        this.object.add(wall);
    }
}
