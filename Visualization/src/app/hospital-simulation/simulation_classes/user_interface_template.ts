import * as THREE from "three";
import { GUI } from "three/addons/libs/lil-gui.module.min.js";

interface TimeSettings {
    hours: number;
}

interface SheduleDay {
    day: string;
}

export default class UserInterface {
    gui: GUI;
    timeSettings: TimeSettings;
    selectedDay : SheduleDay;

    constructor(scene: THREE.Scene, renderer: THREE.WebGLRenderer, document : any, scheduledDays : string[]) {
        // Helper function to update object color
        const colorCallback = (object: THREE.Light, color: string): void => {
            object.color.set(color);
        };

        this.gui = new GUI({ hideable: false } as any);
        //, container: document.getElementById('parent')
        const guiDomElement = this.gui.domElement;
        guiDomElement.style.position = 'absolute';
        guiDomElement.style.right = '45px';
        guiDomElement.style.top = '237px';
        guiDomElement.style.zIndex = '1';

        // Create the lights folder
        const lightsFolder = this.gui.addFolder("Lights");

        // Create the ambient light folder
        const ambientLightFolder = lightsFolder.addFolder("Ambient Light");

        // Create an ambient light instance
        const ambientLight = new THREE.AmbientLight(0xffffff, 0.5); // Default white light with medium intensity
        scene.add(ambientLight); // Assuming `scene` is your Three.js scene

        // Add GUI controls for the ambient light
        const ambientColor = { color: `#${ambientLight.color.getHexString()}` };
        ambientLightFolder.addColor(ambientColor, "color").onChange(color => colorCallback(ambientLight, color));
        ambientLightFolder.add(ambientLight, "intensity", 0.0, 1.0, 0.01);

        // Create the directional light folder
        const directionalLightFolder = lightsFolder.addFolder("Directional Light");

        // Create a directional light instance
        const directionalLight = new THREE.DirectionalLight(0xffffff, 1); // Default white light with full intensity
        directionalLight.castShadow = true; // Enable shadows for the light
        directionalLight.position.set(10, 10, 15); // Set initial position
        
        // Set shadow properties for better performance and quality
        directionalLight.shadow.mapSize.width = 2048;  // Shadow map resolution (increase for better quality)
        directionalLight.shadow.mapSize.height = 2048; // Shadow map resolution (increase for better quality)
        directionalLight.shadow.camera.near = 5;
        directionalLight.shadow.camera.far = 50;
        directionalLight.shadow.camera.left = -20;
        directionalLight.shadow.camera.right = 20;
        directionalLight.shadow.camera.top = 10;
        directionalLight.shadow.camera.bottom = -10;
        directionalLight.shadow.radius = 4;  // Larger value for softer shadows
        directionalLight.shadow.bias = -0.00005;

        scene.add(directionalLight); // Add the light to the scene
    
        // Object for GUI controls
        const directionalLightSettings = {
            color: `#${directionalLight.color.getHexString()}`,
        };

        // Add GUI controls for the directional light
        directionalLightFolder.addColor(directionalLightSettings, "color").onChange(color => {
            directionalLight.color.set(color);
        });
        directionalLightFolder.add(directionalLight, "intensity", 0.0, 2.0, 0.1); // Intensity control
        directionalLightFolder.add(directionalLight.position, "x", 0, 20, 0.1);
        directionalLightFolder.add(directionalLight.position, "y", 0, 20, 0.1);
        directionalLightFolder.add(directionalLight.position, "z", -20, 20, 0.1);

        // Create the working day time folder
        this.timeSettings = { hours: 0 }; // Starting at 0 (midnight)
        this.selectedDay = { day : scheduledDays[0]};
        const timeFolder = this.gui.addFolder("Schedule Day");

        const dayControl = timeFolder.add(this.selectedDay, 'day', scheduledDays);
        dayControl.listen();
        const daySelect = dayControl.domElement.querySelector('select');
        if (daySelect) {
            daySelect.style.fontSize = '14px';
        }
        
        timeFolder.add(this.timeSettings, "hours", 0, 24, 0.5);
    }

    setVisibility(visible: boolean): void {
        if (visible) {
            this.gui.show();
        } else {
            this.gui.hide();
        }
    }
}
