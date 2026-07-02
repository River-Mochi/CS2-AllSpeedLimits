// File: UI/src/entry/SpeedToolToolbarButton.tsx
// Purpose: Game Top Left button for opening the road/rail speed tool.

import { Button, Tooltip } from "cs2/ui";
import { SetToolActive, TOOL_ACTIVE } from "../shared/bindings";
import classNames from "classnames";
import styles from "./SpeedToolToolbarButton.module.scss";
import iconSvg from "../images/icon-speedlimit-30Gray02.svg";
import { useText } from "../shared/localization";
import { useSafeBinding } from "../shared/useSafeBinding";

export const SpeedToolToolbarButton = () => {
    const TEXT = useText().inCity;
    const toolActive = useSafeBinding(TOOL_ACTIVE, false);

    const handleClick = () => {
        // Send the explicit desired state instead of a blind toggle. Combined with the idempotent
        // ToggleTool in C#, this stays correct even if onSelect fires twice for one click.
        SetToolActive(!toolActive);
    };

    return (
        <Tooltip tooltip={TEXT.toolbar.tooltip}>
            <Button
                variant="floating"
                className={classNames({ [styles.selected]: toolActive }, styles.toggle)}
                onSelect={handleClick}
            >
                <img 
                    src={iconSvg}
                    style={{ 
                        width: "100%",
                        height: "100%"
                    }} 
                />
            </Button>
        </Tooltip>
    );
};
