class Transport:
    def __init__(self, longitude, latitude, maxSpeed, startSpeed=0, type_="car"):
        self.__longitude = longitude
        self.__latitude = latitude
        self.__type_ = type_
        self.__direction = {"from": None,"to": None}
        self.__speed = startSpeed
        self.__maxSpeed = maxSpeed

    def move(self):
        pass #залупа