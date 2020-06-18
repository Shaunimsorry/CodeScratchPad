#Import Dependancies
import os
import sys
import random
import time

#Start Clock
startTime = time.time();

##Run Main Loop
wordsList = []
ranInt = 0;
userScore = 0;
tgtScore = 50;

#Put In All the problem words
wordsList.append("それ")
wordsList.append("あれ")
wordsList.append("これ")
wordsList.append("その")
wordsList.append("あの")
wordsList.append("この")

#Chose a random word
while userScore < tgtScore:
    ranInt = random.randrange(0,len(wordsList))
    print ("Random Particle Is: "+wordsList[ranInt])
    userAnswer = input("Your Answer: ")
    if userAnswer == wordsList[ranInt]:
        print("Yes")
        userScore = userScore + 1
        print (userScore)
    else:
        print("No")
        userScore = userScore - 1
        print (userScore)

print ("Time Taken To Reach TargetScore: "+str(time.time() - startTime)+" Seconds")


