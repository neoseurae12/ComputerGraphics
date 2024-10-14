#version 330 core
out vec4 FragColor;

// my code

struct Material {
    sampler2D diffuse;
    sampler2D specular;    
    float shininess;
}; 

struct Light {
    vec3 position;  
    // angular
    vec3 direction;   
    float cutOff;
    float outerCutOff;  //
  
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
	
    float constant;
    float linear;
    float quadratic;
};

//

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
} fs_in;


uniform sampler2D floorTexture;
uniform vec3 lightPos;    // hw5_my code - erase
uniform vec3 viewPos;
uniform vec3 lightColor;    // hw5_my code - erase
uniform vec3 ObjColor;

// my code
uniform Material material;
uniform Light light;
uniform bool angAttn;   // toggle
//

void main()
{           
    // should check    


    vec3 color = texture(floorTexture, fs_in.TexCoords).rgb;

    // ambient
    //vec3 ambient = 0.1 *  color;
    vec3 ambient = light.ambient * color;
    

    // diffuse
    //vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    vec3 lightDir = normalize(light.position - fs_in.FragPos);
    vec3 normal = normalize(fs_in.Normal);
    //float diff = max(dot(lightDir, normal), 0.0);
    //float diff = max(dot(light.direction, normal), 0.0);
    float diff = max(dot(lightDir, normal), 0.0);
    //vec3 diffuse = diff * color;
    vec3 diffuse = light.diffuse * diff * color;
    
    // specular
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    //vec3 reflectDir = reflect(-lightDir, normal);
    //vec3 reflectDir = reflect(-light.direction, normal);
    vec3 reflectDir = reflect(-lightDir, normal);
    //float spec = 0.0;
    //const float specularStrength = 0.5;
    vec3 halfwayDir = normalize(light.direction + viewDir);
    //spec = pow(max(dot(normal, halfwayDir), 0.0), 32); //32
    //spec = pow(max(dot(normal, halfwayDir), 0.0), material.shininess);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = light.specular * spec * color;

    // my code

    if (angAttn) {
        // spotlight (soft edges)
        float theta = dot(lightDir, normalize(-light.direction)); 
        float epsilon = (light.cutOff - light.outerCutOff);
        float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
        diffuse  *= intensity;
        specular *= intensity;
    }
    

    // attenuation
    //float distance    = length(light.position - FragPos);
    float distance    = length(light.position - fs_in.FragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    
    ambient  *= attenuation; 
    diffuse   *= attenuation;
    specular *= attenuation;
    //
    
    //vec3 specular = specularStrength * lightColor * spec; 
    //vec3 specular = specularStrength * light.specular * spec; 
    FragColor = vec4(ambient + diffuse + specular, 1.0);
}