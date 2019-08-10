/*This is a block quote and they have said this should be on the top of each file


*/

#pragma once;

#include "FBullCowGame.h"
#include <map>
#define TMap std::map;
using int32 = int;
using FString = std::string;


FBullCowGame::FBullCowGame()
{
	Reset();
}

void FBullCowGame::Reset() 
{
	const FString HIDDEN_WORD = "planet";
	MyHiddenWord = HIDDEN_WORD;
	constexpr int32 MAX_TRIES = 5;
	MyMaxTries = MAX_TRIES;
	MyCurrentTry = 0;
	bGamewon = false;
	return; 
}

int32 FBullCowGame::GetMaxTries() const 
{
	return MyMaxTries;
}

int32 FBullCowGame::GetCurrentTry()
{
	return MyCurrentTry;
}

bool FBullCowGame::IsGameWon() const 
{
	return bGamewon;
}

bool FBullCowGame::IsGuessValid (FString) 
{
	return false;
}

//Assuming Valid Input Came In
FBullCowCount FBullCowGame::SubmitGuess(FString input)
{
	MyCurrentTry++;
	FBullCowCount BullCownCount;
	for (int32 i = 0; i < MyHiddenWord.length(); i++)
	{
		if (MyHiddenWord[i] != input[i])
		{
			BullCownCount.Cows++;
		}
		for (int32 j = 0; j < input.length(); j++)
		{
			if (MyHiddenWord[i] == input[j])
			{
				BullCownCount.Bulls++;
			}
		}
	}

	if (BullCownCount.Bulls == MyHiddenWord.length())
	{
		bGamewon = true;
	}
	else
	{
		bGamewon = false;
	}

	return BullCownCount;
}

FString FBullCowGame::checkHiddenWord()
{
	return MyHiddenWord;
}

int32 FBullCowGame::GetHiddenWordLen() const
{
	return MyHiddenWord.length();
}

EWordStatus FBullCowGame::checkIsGuessValid(FString guess) const
{
	if (guess.length() != GetHiddenWordLen())
	{
		return EWordStatus::Wrong_Length;
	}
	if (guess.length() == GetHiddenWordLen())
	{
		return EWordStatus::Ok;
	}
}
