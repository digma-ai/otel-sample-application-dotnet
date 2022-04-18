package user

import (
	"errors"
	"time"
)

var (
	ExtraLatency time.Duration
)

type Service interface {
	List() ([]User, error)
	Add(user User) error
	Get(id string) (User, error)
	Init()
}

func NewUserService() Service {
	return &userService{}
}

type User struct {
	Id   string `json:"id"`
	Name string `json:"name"`
}

type userService struct {
	users map[string]User
}

var ErrIdInvalid = errors.New("user id too long")
var ErrUserAlreadyExists = errors.New("user already exists")
var ErrUserNotFound = errors.New("user not found")

func (u *userService) Init() {
	u.users = make(map[string]User)
}

func (u *userService) Get(id string) (User, error) {
	time.Sleep(ExtraLatency)
	value, found := u.users[id]
	if !found {
		panic(ErrUserNotFound)
	}
	return value, nil
}

func (u *userService) List() ([]User, error) {
	time.Sleep(ExtraLatency)

	v := []User{}
	for _, value := range u.users {
		v = append(v, value)
	}
	return v, nil
}

func (u *userService) Add(user User) error {
	time.Sleep(ExtraLatency)
	if len(user.Id) > 5 {
		return ErrIdInvalid
	}
	if _, ok := u.users[user.Id]; ok {
		return ErrUserAlreadyExists
	}

	u.users[user.Id] = user
	return nil
}
