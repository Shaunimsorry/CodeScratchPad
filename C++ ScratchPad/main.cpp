/* 
THIS IS THE CONSOLE EXECUTABLE THAT MAKES USE OF THE BALLCOW CLASS
THIS ACTS AS THE VIEW IN A MVC PATTERN AND IS REPONSIBLE FOR ALL USER INTERACTION FOR
GAME LOGIC SEE FBULLCOWGAME CLASS
*/



#include <iostream> //including the io streams
#include <string>   //string library ?
#include "FBullCowGame.h";

using FText = std::string;
using int32 = int;

void intro();
void PlayGame();
FText getGuess();
bool askToPlayAgain();
FBullCowGame BCGame;

int main()
{
    intro();
	PlayGame();
	askToPlayAgain();
    return 0;
}

void intro()
{
	std::cout << "Welcome To The C++ Tutorial" << std::endl;
	std::cout << "Current Hidden Word Length Is: " << BCGame.GetHiddenWordLen();
	std::cout << std::endl;
	std::cout << "Guess The Word Im Thinking Of";
	return;
}

void PlayGame()
{
	// TODO go back home
	BCGame.Reset();
	while (!BCGame.IsGameWon() && BCGame.GetCurrentTry() <= BCGame.GetMaxTries())
	{
		FText guess = getGuess();
	}

}

FText getGuess()
{
	FText guess = "";
	FBullCowCount _bullCowCount;
	std::cout << std::endl;
	std::cout << "Attempt: " << BCGame.GetCurrentTry() << std::endl;
	std::getline(std::cin, guess);
	EWordStatus checkGuess = BCGame.checkIsGuessValid(guess);
	switch (checkGuess)
	{
	case EWordStatus::Ok:
		_bullCowCount = BCGame.SubmitGuess(guess);
		std::cout << "Bulls = " << _bullCowCount.Bulls;
		std::cout << std::endl;
		std::cout << "Cows = " << _bullCowCount.Cows;
		break;
	case EWordStatus::Wrong_Length:
		std::cout << "Word Is Wrong Size Please Retry";
		break;
	default:
		return guess;
	}
	return guess;
}

bool askToPlayAgain()
{
	std::cout << std::endl; 
	std::cout << "Type Y To play again!, Thanks for playing!!!" << std::endl;
	FText reponse = "";
	std::getline(std::cin, reponse);
	return false;
}
