struct LightInfo {  
    vec4 Position; // Light position in eyecoords.  
    vec3 La; // Ambient light intensity  
    vec3 Ld; // Diffuse light intensity  
    vec3 Ls; // Specular light intensity  
};  
uniform LightInfo Light; 
