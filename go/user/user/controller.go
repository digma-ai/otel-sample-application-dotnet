package user

import (
	"encoding/json"
	"fmt"
	"io/ioutil"
	"net/http"

	"github.com/gorilla/mux"
)

type UserController struct {
	Service Service
}

func (controller *UserController) Get(w http.ResponseWriter, req *http.Request) {
	userId := mux.Vars(req)["id"]
	user, _ := controller.Service.Get(userId)
	json.NewEncoder(w).Encode(user)
}

func (controller *UserController) Add(w http.ResponseWriter, req *http.Request) {
	var user User
	reqBody, err := ioutil.ReadAll(req.Body)
	if err != nil {
		fmt.Fprintf(w, "please enter user details")
	}
	json.Unmarshal(reqBody, &user)
	error := controller.Service.Add(user)
	if error != nil {
		w.WriteHeader(http.StatusBadRequest)
		json.NewEncoder(w).Encode(error.Error())
	} else {
		w.WriteHeader(http.StatusCreated)
		json.NewEncoder(w).Encode(user)
	}

}

func (controller *UserController) All(w http.ResponseWriter, req *http.Request) {
	users, _ := controller.Service.List()
	json.NewEncoder(w).Encode(users)
}
