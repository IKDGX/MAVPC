package com.example.mavpc.modelos;

public class CamFavorita {
    private String id;
    private String userId;
    private String camId;

    public CamFavorita(String id, String userId, String camId) {
        this.id = id;
        this.userId = userId;
        this.camId = camId;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getUserId() {
        return userId;
    }

    public void setUserId(String userId) {
        this.userId = userId;
    }

    public String getCamId() {
        return camId;
    }

    public void setCamId(String camId) {
        this.camId = camId;
    }
}