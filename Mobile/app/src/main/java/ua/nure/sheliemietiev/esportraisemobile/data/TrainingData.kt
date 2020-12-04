package ua.nure.sheliemietiev.esportraisemobile.data

import ua.nure.sheliemietiev.esportraisemobile.data.TeamMember
import ua.nure.sheliemietiev.esportraisemobile.data.VideoStreamItem

class TrainingData(
    val teamMembers: Map<Int, TeamMember>,
    val videoStreams: Iterable<VideoStreamItem>
)