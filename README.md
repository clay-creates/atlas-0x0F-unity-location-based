# AR Geolocation Navigation App

## **Overview**
This application is a **simulation and navigation tool** built with **Unity, Niantic Lightship VPS, and GPS services**. It enables users to track their location, save coordinates, calculate distances, and instantiate **AR map markers** for custom navigation.  

The app integrates **Niantic Lightship VPS** (Visual Positioning System), allowing for more **precise, localized AR experiences** in pre-scanned locations. If VPS is unavailable, the app **defaults to GPS-based navigation**, making it useful in various outdoor and real-world environments.

---

## **Context and Applications**
The **core purpose** of this application is to explore **AR-powered navigation using geolocation data**.  
Since this app works alongside **Niantic Lightship VPS**, its scope is **more local than global**—meaning it is intended to function in **specific VPS-mapped areas** rather than across the entire world.

However, when **GPS is available**, the app provides a **custom navigation experience**:
- Users can **track their location** and measure how far they've traveled.
- **Save places they’ve been** and **mark key locations** with AR map markers.
- **Use markers for custom routes**, which could be useful for hiking, city exploration, or any scenario requiring **spatial awareness**.

---

## **Geolocation Tools and Technologies**
This application leverages **several geolocation and AR tools**:

- **Niantic Lightship VPS** (via **Niantic Lightship ARDK 3**)  
  - Enables **precise, persistent AR object placement** in VPS-mapped locations.
- **Unity Location Services** (Built-in GPS Tracking)  
  - Provides **real-time GPS data** when VPS is unavailable.
- **Unity 2022.3.7f1**
- **Google ARCore XR Plugin**
- **AR Foundation** (for handling AR tracking and rendering)
- **GPSEncoder** (3rd-party script for GPS-to-Unity coordinate conversion)

#### **Challenges in Geolocation-AR Integration**
Integrating **GPS tracking with AR frameworks** has been an **interesting challenge**.  
- While **AR tools seem designed for use with GPS**, detailed documentation on **pulling GPS data** via **Niantic Lightship** has been **scarce**.
- Due to this, the app currently **uses Unity’s built-in Location Service** and **custom scripts** to determine if a user is within a VPS-enabled area.  
  - If VPS **is available**, the app switches to using VPS localization.
  - If VPS **is not available**, the app defaults to GPS tracking.

---

## **Implementation Details**
The core of this application revolves around **GPS tracking and AR marker placement**:

### **🗺️ Core Features**
✅ **Track and Save GPS Locations**  
- Retrieve **current GPS coordinates** and display them in **Unity UI**.  
- **Save locations** for later reference.  

✅ **Calculate Distances Between Locations**  
- Measure the **distance between saved locations** and current position.  

✅ **Instantiate AR Markers at Saved & Current Locations**  
- Users can **place markers** in AR space based on their **real-world GPS location**.  
- **Two markers are instantiated**:  
  - One at the **saved GPS location**  
  - One at the **current GPS location**  

✅ **Niantic Lightship VPS Integration (Work in Progress)**  
- Detects if a user is within a **VPS-enabled area**.  
- Uses **VPS localization** for **more precise marker placement** and **persistent AR objects**.

✅ **Future Enhancements**
- **Persistent Route Mapping:** Save and visualize previous paths.  
- **Dynamic Navigation:** Use saved markers to **guide users back** along the same route.  
- **Lightship VPS Mesh Integration:** Dynamically adjust GPS precision when a VPS scan is detected.

---

## **🚀 Current Progress & Testing**
### **🛠️ GPS & Location Services**
✅ GPS data is **accurately retrieved and displayed** in real-time.  
✅ Coordinates **update dynamically** as the user moves.  
✅ Distance calculations are **working as expected**.  
✅ Markers **spawn correctly**.

### **🔬 VPS Integration**
⚠️ **Currently testing Lightship VPS integration**.  
⚠️ VPS location recognition is **in development**.  
⚠️ Issues with connecting VPS scans to **dynamic object placement**.

### **📱 Android Compatibility**
✅ Successfully tested on **Android devices**.  
⚠️ **iOS build not yet implemented** (would require permission request adjustments).  
⚠️ Lightship VPS integration **may behave differently on iOS**, further testing required.

---

## **🔮 Future Vision**
I believe that **AR and geolocation technologies** **complement each other well**, especially considering the **rise of Geo-AR applications** like Pokémon GO.  

### **🔹 Possible Future Features**
- **Expanded GPS Navigation**:  
  - Generate **custom paths** based on saved locations.  
  - Enable a **direction visualizer** to help users retrace their steps.  
- **More Advanced VPS Integration**:  
  - Use **Lightship VPS to improve AR tracking accuracy**.  
  - Allow users to **scan and save VPS-mapped locations** dynamically.  
- **Cross-Platform Compatibility**:  
  - Implement **iOS support** with custom permissions.  
  - Ensure **consistent marker placement** across devices.

---

## **📌 Installation & Setup**
### **🔧 Prerequisites**
- **Unity 2022.3.7f1** (or later)
- **Niantic Lightship ARDK 3**
- **Google ARCore XR Plugin**
- **AR Foundation**
- **Android Build Support** (for testing on an actual device)

### **📥 Clone the Repository**
```sh
git clone https://github.com/YOUR_GITHUB_USERNAME/clay-creates/atlas-0x0F-unity-location-based.git
