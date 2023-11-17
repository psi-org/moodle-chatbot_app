
# Moodle Chatbot App

+ Version: 1.0.0 (Sep 05/ 2023)

+ Copyright: Population Services International – [psi.org](https://www.psi.org/)

+ License: [GNU General Public License, Version 3](http://www.gnu.org/licenses/gpl-3.0.html)

+ Maintained by: [PSI’s Digital Health Management](https://www.psi.org/practice-area/digital-health/)

## Summary

The Moodle Chatbot Application allows users to take and complete Moodle courses entirely from a chatbot client like WhatsApp.

The chatbot app was originally designed to support Low or Medium Income Countries (LMIC) learners who may not have access to traditional educational forums or access to broadband and a fully featured mobile device. It was developed to support community health workers, case managers and other front line health workers to provide them with critical knowledge and skills, building through an accessible and familiar app in their context, in most cases WhatsApp. 
The application has gone through multiple rounds of field-testing with end users to ensure that it meets usability requirements for its adoption.

## What does it do?

The Moodle Chatbot App enables on a familiar chatbot conversation all the steps necessary to complete an activity-based Moodle course. It guides the user through the Moodle user account creation, enrolls users in available chatbot-compatible courses, and finally allows the completion of 3 types of Moodle course activities: lesson, quiz, and feedback. In other words, users do not need to use the web-based or android versions of Moodle, and instead can use a familiar interface like WhatsApp, that for certain learner cohorts in LMIC countries, has proved to be more accessible and effective.

The Moodle Chatbot App manages the conversation with the user, it is responsible for listing compatible Moodle courses and doing enrollment and rendering course activities in a chatbot conversation format via WhatsApp or other chatbot platforms. The app interacts with Moodle using the native API, as well as a supplemental set of end points, which are implemented as a Moodle plugin.

Further information about the companion **_Moodle Chatbot Plugin_** and the Moodle Chatbot App can be found here:
https://psi.atlassian.net/wiki/spaces/MoodleChatbot/overview

## Requirements

+ To run source code, .Net Core SDK version 3.1 or above is required, alternatively, instructions on how to create a docker container are provided [here](https://psi.atlassian.net/wiki/spaces/MoodleChatbot/pages/167117198/Chatbot+App+Deployment).
+ Twilio account for WhatsApp.
+ Moodle instance configured to work with the chatbot (learn how [here](https://psi.atlassian.net/wiki/spaces/MoodleChatbot/pages/167018940/Moodle+Course+Setup)).
+ For a development environment, we recommend the use of Ngrok to expose localhost on the internet, so Twilio will be able to reach the chatbot on your local machine.

## Features

### User Registration
+ Checks if the telephone number is new on the platform. If available, it starts the registration process.
+ Automatically sets the user language based on the dialing code (only for some African countries for now). Languages currently  available: English, Portuguese and French.
+ Only required fields are first name and last name, with the email being optional.


### Course Display and Self Enrollment
+ List chatbot-compliant courses
+ Self enroll a user on any available course
+ Track user's course progress and completion status 

### Course Activities Completion
Navigate through the course modules using a menu designed for the chatbot environment.

#### Lesson
+ Configure a lesson activity that will properly display its pages for the WhatsApp interface.
+ Compatible with .jpg images and .mp4 videos (for public courses only).
+ Lesson summary after finishing the module.

#### Quiz
+ Quizzes consisting of ‘multiple choice’ and ‘true/false’ questions can be taken using the Moodle Chatbot App.
+ Multiple attempts supported.
+ Quiz summary after attempt submission.

#### Feedback
+ Get feedback from the students using Moodle’s feedback module, compatible with ‘multiple choice’ and ‘short text answer’ type questions

## Moodle Configuration

Please refer to the [Moodle Course Setup](https://psi.atlassian.net/wiki/spaces/MoodleChatbot/pages/167018940/Moodle+Course+Setup) page for detailed information on how to configure a Moodle course to ensure its correct functionality on the Moodle Chatbot App.
## Installation

Please refer to the [Installation](https://psi.atlassian.net/wiki/spaces/MoodleChatbot/pages/167117177/Installation) page for detailed information on how to install the Moodle Chatbot App and all its dependencies.

## Troubleshooting, Bugs, and Feedback

Submit any bugs or feedback at [Moodle Chatbot App Issues](https://github.com/psi-org/moodle-chatbot_app/issues)
